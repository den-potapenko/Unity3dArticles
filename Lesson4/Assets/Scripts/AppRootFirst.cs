//
// Author: Denis Potapenko
// http://denis-potapenko.blogspot.com/
// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppRootFirst : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    // rects for rendering gui elements
    private readonly Rect cSendHelloRect = new Rect(20, 200, 200, 200);
    private readonly Rect cSendDataRect = new Rect(240, 200, 200, 200);
    private readonly Rect cDebugMsgRect = new Rect(20, 420, 200, 100);

    // debug message and data
    private string mSendMessage = "No messages";
    private byte[] mDataToSend = new byte[] { 0x0b, 0x1f, 0x3c };

    // timer to create some delay for sending messages
    private float mWaitTimeUpdate = 0.0f;
    private const float cMaxWaitTimeUpdate = 0.1f;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface
    public void Start()
    {
        //
        InitNet();
    }

    public void Update()
    {
        
    }

    public void OnGUI()
    {
        // increment wait time
        if (mWaitTimeUpdate < cMaxWaitTimeUpdate)
        {
            mWaitTimeUpdate += Time.deltaTime;
        }
        

        if (GUI.Button(cSendHelloRect, "Hello"))
        {
            if (mWaitTimeUpdate > cMaxWaitTimeUpdate)
            {
                //
                this.networkView.RPC(Constants.cRPCSendMessage, Constants.cSendMessagesMode, "Hello");

                //
                mWaitTimeUpdate = 0.0f;

                //
                mSendMessage = "'Hello' message sent";
            }
        }

        if (GUI.Button(cSendDataRect, "Send data"))
        {
            if (mWaitTimeUpdate > cMaxWaitTimeUpdate)
            {
                //
                this.networkView.RPC(Constants.cRPCSendData, Constants.cSendMessagesMode, mDataToSend);

                //
                mWaitTimeUpdate = 0.0f;

                //
                mSendMessage = "Data sent";
            }
        }

        GUI.Label(cDebugMsgRect, mSendMessage);
    }

    ///////////////////////////////////////////////////////////////////////////
    #region RPC functions

    [RPC]
    public void RPCSendMessage(string msg)
    {

    }

    [RPC]
    public void RPCSendData(byte[] data)
    {

    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    /// <summary>
    /// Initializes RPC server
    /// </summary>
    private void InitNet()
    {
        Network.InitializeServer(3, Constants.cServerPort, false);
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
