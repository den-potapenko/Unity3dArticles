using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class TextureLoader
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    private const string cLocalPathPrefix = "file://localhost/";
    private const int cMaxTexSize = 64;

    private static TextureLoader mInstance = null;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface

    private TextureLoader() { }

    ///////////////////////////////////////////////////////////////////////////
    #region LoadSyncTexture2D
    public Texture2D LoadSyncPNGTexture2D(string path)
    {
        string resPath = cLocalPathPrefix + path;
        Debug.Log("Try to load texture at: " + resPath);

        WWW www = new WWW(resPath);
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning("Error! '" + www.error + "'");
            return null;
        }
        bool texChecked = false;

        while (!www.isDone)
        {
            if (!texChecked)
            {
                float texWidth = 0;
                float texHeight = 0;
                GetPNGTextureSize(www.bytes, out texWidth, out texHeight);

                if (texWidth < 0 || texHeight < 0)
                {
                    continue;
                }

                texChecked = true;
            }

        };

        //Debug.Log("error = " + www.error);
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogWarning("Error! '" + www.error + "'");
            return null;
        }

        // get texture
        Texture2D tex = www.texture;

        return tex;
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region GetPNGTextureSize

    private void GetPNGTextureSize(byte[] bytes, out float width, out float height)
    {
        width = -1.0f;
        height = -1.0f;

        // check only png tex!!! // http://www.libpng.org/pub/png/spec/1.2/PNG-Structure.html
        byte[] png_signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
        const int cMinDownloadedBytes = 30;

        byte[] buf = bytes;
        if (buf.Length > cMinDownloadedBytes)
        {
            // now we can check png format
            for (int i = 0; i < png_signature.Length; i++)
            {
                if (buf[i] != png_signature[i])
                {
                    Debug.LogWarning("Error! Texture os NOT png format!");
                    return; // this is NOT png file!
                }
            }

            // now get width and height of texture
            width = buf[16] << 24 | buf[17] << 16 | buf[18] << 8 | buf[19];
            height = buf[20] << 24 | buf[21] << 16 | buf[22] << 8 | buf[23];

            Debug.Log("Loaded texture size: width = " + width + "; height = " + height);
            return;
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    /// <summary>
    /// Gets instance of the object
    /// </summary>
    public static TextureLoader Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new TextureLoader();
            }

            return mInstance;
        }
    }

    #endregion
}
