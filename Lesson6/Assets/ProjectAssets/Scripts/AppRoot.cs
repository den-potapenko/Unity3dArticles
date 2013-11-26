using UnityEngine;
using System.Collections;
using System.IO;

using System.Runtime;
using System.Runtime.InteropServices;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using NAudio;
using NAudio.Wave;

public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    private static AppRoot mInstance = null;

    private const string cLocalPath = "file://localhost/";

    private IWavePlayer mWaveOutDevice;
    private WaveStream mMainOutputStream;
    private WaveChannel32 mVolumeStream;
    
    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface

    public AppRoot()
    {
        mInstance = this;
    }

    public void Start()
    {
    }

    public void Update()
    {

    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(100, Screen.height - 200 + 100, Screen.width - 200, 35), "Load audio"))
        {
            UnloadAudio();
            LoadAudio();
        }
    }

    public void OnApplicationQuit()
    {
        UnloadAudio();
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation
    
    private void LoadAudio()
    {
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ofd.Title = "Open audio file";
        ofd.Filter = "MP3 audio (*.mp3) | *.mp3";
        if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            WWW www = new WWW(cLocalPath + ofd.FileName);
            Debug.Log("path = " + cLocalPath + ofd.FileName);
            while (!www.isDone) { };
            if (!string.IsNullOrEmpty(www.error))
            {
                System.Windows.Forms.MessageBox.Show("Error! Cannot open file: " + ofd.FileName + "; " + www.error);
                return;
            }

            byte[] imageData = www.bytes;

            if (!LoadAudioFromData(imageData))
            {
                System.Windows.Forms.MessageBox.Show("Cannot open mp3 file!");
                return;
            }
            
            mWaveOutDevice.Play();

            Resources.UnloadUnusedAssets();
        }

    }

    private bool LoadAudioFromData(byte[] data)
    {
        try
        {
            MemoryStream tmpStr = new MemoryStream(data);
            mMainOutputStream = new Mp3FileReader(tmpStr);
            mVolumeStream = new WaveChannel32(mMainOutputStream);

            mWaveOutDevice = new WaveOut();
            mWaveOutDevice.Init(mVolumeStream);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error! " + ex.Message);
        }

        return false;
    }

    private void UnloadAudio()
    {
        if (mWaveOutDevice != null)
        {
            mWaveOutDevice.Stop();
        }
        if (mMainOutputStream != null)
        {
            // this one really closes the file and ACM conversion
            mVolumeStream.Close();
            mVolumeStream = null;

            // this one does the metering stream
            mMainOutputStream.Close();
            mMainOutputStream = null;
        }
        if (mWaveOutDevice != null)
        {
            mWaveOutDevice.Dispose();
            mWaveOutDevice = null;
        }

    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    private static AppRoot Instance
    {
        get
        {
            return mInstance;
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
