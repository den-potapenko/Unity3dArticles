//
// Author: Denis Potapenko
// http://denis-potapenko.blogspot.com/
// 
using UnityEngine;
using System.Collections;

public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    private Texture2D mLoadedTexture;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface

    // Use this for initialization
    public void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(100, Screen.height - 100, Screen.width - 200, 35), "Load audio"))
        {
            LoadTexture();            
        }

        if (mLoadedTexture != null)
        {
            GUI.Button(new Rect(100, Screen.height - 500, 350, 350), mLoadedTexture);
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    private void LoadTexture()
    {
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ofd.Title = "Open png file";
        ofd.Filter = "PNG image (*.png) | *.png";
        if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Texture2D texLoaded = TextureLoader.Instance.LoadSyncPNGTexture2D(ofd.FileName);

            if (texLoaded != null)
            {
                mLoadedTexture = texLoaded;
            }
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
