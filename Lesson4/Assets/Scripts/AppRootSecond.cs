//
// Author: Denis Potapenko
// http://denis-potapenko.blogspot.com/
// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppRootSecond : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    // rect for displaying of received message
    private readonly Rect cMsgRect = new Rect(20, 420, 200, 100);

    // received message
    private string mReceiveMessage = "No messages";

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface
    public void Start()
    {
        InitNet();
    }

    public void Update()
    {
        
    }

    public void OnGUI()
    {
        // just show received message
        GUI.Label(cMsgRect, mReceiveMessage);
    }

    #region RPC functions

    [RPC]
    public void RPCSendMessage(string msg)
    {
        mReceiveMessage = "Message received = " + msg;
    }

    [RPC]
    public void RPCSendData(byte[] data)
    {
        mReceiveMessage = "Data received. Data length = " + data.Length;
    }

    #endregion

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    private void InitNet()
    {
        //
        Network.Connect(Constants.cServerIp, Constants.cServerPort);
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
