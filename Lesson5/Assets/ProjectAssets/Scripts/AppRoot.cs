using UnityEngine;
using System.Collections;
using System.IO;

using System.Runtime;
using System.Runtime.InteropServices;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    private static AppRoot mInstance;

    public Texture2D mResTexture = null;

    private const string cLocalPath = "file://localhost/";

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface

    public AppRoot()
    {
        mInstance = this;

        //EditorUtility.OpenFilePanel()
    }

    public void Start()
    {
    }

    private Texture2D LoadImageFromData(byte[] imageData)
    {
        const int cNumImages = 1;
        uint[] handles = new uint[cNumImages];
        Texture2D resTexture = null;

        /* First we initialize the library. */
        /*Do not forget that... */
        DevILLoader.ilInit();

        /* We want all images to be loaded in a consistent manner */
        DevILLoader.ilEnable(DevILConstants.IL_ORIGIN_SET);

        /* In the next section, we load one image */
        DevILLoader.ilGenImages(cNumImages, handles);
        DevILLoader.ilBindImage(handles[0]);

        //
        //uint res = PluginLoader.ilLoadL(DevILConstants.IL_PNG, imageData, (uint)imageData.Length);
        bool res = DevILLoader.ilLoadL(DevILConstants.IL_TYPE_UNKNOWN, imageData, (uint)imageData.Length);
        if (!res)
        {
            Debug.LogWarning("Error! Cannot load image from data");
            return resTexture;
        }

        /* Let's spy on it a little bit */
        int width = DevILLoader.ilGetInteger(DevILConstants.IL_IMAGE_WIDTH); // getting image width
        int height = DevILLoader.ilGetInteger(DevILConstants.IL_IMAGE_HEIGHT); // and height
        Debug.Log("Base image resolution: w = " + width + "; h = " + height);

        // create result texture
        resTexture = new Texture2D(width, height, TextureFormat.RGBA32, true);

        //
        Color32[] texColors = GetColorDataFromCurrentImage();
        
        // set first mip map
        resTexture.SetPixels32(texColors, 0);

        // now, try to set another levels of bitmap
        {
            uint currMipMapLevel = 1;
            const uint cMaxMipMapLevel = 15;
            while (currMipMapLevel < cMaxMipMapLevel)
            {
                res = DevILLoader.ilActiveMipmap(currMipMapLevel);
                Debug.Log("res = " + res + " currMipMapLevel = " + currMipMapLevel);
                if (!res)
                {
                    break;
                }

                //
                texColors = GetColorDataFromCurrentImage();

                Debug.Log("currMipMapLevel = " + currMipMapLevel);

                // set next mip map
                resTexture.SetPixels32(texColors, (int)currMipMapLevel);

                ++currMipMapLevel;

                // restore base image
                DevILLoader.ilBindImage(handles[0]);
            }
        }

        resTexture.Apply(true);
        //resTexture.Apply(false); // show this to ven!

        /* Finally, clean the mess! */
        DevILLoader.ilDeleteImages(cNumImages, handles);

        return resTexture;
    }

    private Color32[] GetColorDataFromCurrentImage()
    {
        int width = DevILLoader.ilGetInteger(DevILConstants.IL_IMAGE_WIDTH); // getting image width
        int height = DevILLoader.ilGetInteger(DevILConstants.IL_IMAGE_HEIGHT); // and height

        Debug.Log("Image resolution: w = " + width + "; h = " + height);

        /* how much memory will we need? */
        int memoryNeeded = width * height * 4;

        /* We multiply by 4 here because we want 4 components per pixel */
        byte[] imageColorData = new byte[memoryNeeded];

        /* finally get the image data */
        DevILLoader.ilCopyPixels(0, 0, 0, (uint)width, (uint)height, 1, DevILConstants.IL_RGBA, DevILConstants.IL_UNSIGNED_BYTE, imageColorData);

        if (imageColorData.Length <= 0)
        {
            return null;
        }

        // create colors from color data
        Color32[] texColors = new Color32[imageColorData.Length / 4];

        for (int i = 0, j = 0; i < imageColorData.Length; i += 4, ++j)
        {
            texColors[j].r = imageColorData[i];
            texColors[j].g = imageColorData[i + 1];
            texColors[j].b = imageColorData[i + 2];
            texColors[j].a = imageColorData[i + 3];
        }

        return texColors;
    }

    public void Update()
    {

    }

    public void OnGUI()
    {
        if (mResTexture != null)
        {
            GUI.Button(new Rect(100, 100, Screen.width - 200, Screen.height - 200), mResTexture);
        }

        if (GUI.Button(new Rect(100, Screen.height - 200 + 100, Screen.width - 200, 35), "Load texture"))
        {
            //
            //WWW www = new WWW("file://localhost/c:/temp/48.png");
            //WWW www = new WWW("file://localhost/c:/temp/1.dds");
            //WWW www = new WWW("file://localhost/c:/temp/2.bmp");

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "Open image file";
            ofd.Filter = "All files (*.*) | *.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WWW www = new WWW(cLocalPath + ofd.FileName);
                Debug.Log("path = " + cLocalPath + ofd.FileName);
                while (www.isDone) { };
                if (!string.IsNullOrEmpty(www.error))
                {
                    System.Windows.Forms.MessageBox.Show("Error! Cannot open file: " + ofd.FileName + "; " + www.error);
                    return;
                }

                byte[] imageData = www.bytes;
                mResTexture = LoadImageFromData(imageData);
                if (mResTexture == null)
                {
                    System.Windows.Forms.MessageBox.Show("Error! Cannot open image from file: " + ofd.FileName);
                }

                Resources.UnloadUnusedAssets();
            }
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties
    public AppRoot Instance
    {
        get
        {
            return mInstance;
        }
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
