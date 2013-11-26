using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables
    
    // rotate / pan / zoom object
    private TransformObject mTransform; // TransformObject implements rotate / pan / zoom
    private GameObject mGOFlat; // GO rotate around
    private const string cGONameFlat = "Flat";

    // hotspots
    private string[] mGORoomsNames = new string[] 
    {
        "Room0",
        "Room1",
        "Room2"
    };
    private List<GameObject> mGORooms = new List<GameObject>();
    private const float cHotspotSizeX = 70;
    private const float cHotspotSizeY = 24;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface
    public void Start()
    {
        // Find cGONameFlat in scene
        mGOFlat = GameObject.Find(cGONameFlat);

        //
        foreach (var item in mGORoomsNames)
        {
            GameObject goRoom = GameObject.Find(item);
            mGORooms.Add(goRoom);
        }

        // instantiate TransformObject and sets its rotate around object
        mTransform = new TransformObject();
        mTransform.SetTransformRotateAround(mGOFlat.transform);
    }

    public void Update()
    {
        mTransform.Update();
    }

    public void OnGUI()
    {
        Rect tmpRect = new Rect();

        // render labels over game objects
        for (int i = 0; i < mGORooms.Count; i++)
        {
            GameObject goRoom = mGORooms[i];

            // get position of room in 3d space
            Vector3 roomPos = goRoom.transform.position;

            // convert room position from 3d space to screen space (2d)
            Vector3 hotSpotPos = Camera.mainCamera.WorldToScreenPoint(roomPos);

            // calculate rect for rendering label
            tmpRect.x = hotSpotPos.x - cHotspotSizeX / 2;
            tmpRect.y = Screen.height - hotSpotPos.y - cHotspotSizeY / 2;
            tmpRect.width = cHotspotSizeX;
            tmpRect.height = cHotspotSizeY;

            // now render label at this point
            GUI.Box(tmpRect, mGORoomsNames[i]);
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    #endregion

}
