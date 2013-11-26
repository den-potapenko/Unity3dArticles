using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    // materials for highlight
    public Material SimpleMat;
    public Material HighlightedMat;

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

    // temp rectangle. It's create to do not re-create a new one on each frame
    private Rect mTmpRect = new Rect();

    // selected GameObject
    private GameObject mSelectedObject;

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

        // process object selection
        if (Input.GetMouseButtonDown(0))
        {
            SelectObjectByMousePos();
        }
    }

    public void OnGUI()
    {
        // render labels over game objects
        for (int i = 0; i < mGORooms.Count; i++)
        {
            GameObject goRoom = mGORooms[i];

            // get position of room in 3d space
            Vector3 roomPos = goRoom.transform.position;

            // convert room position from 3d space to screen space (2d)
            Vector3 hotSpotPos = Camera.mainCamera.WorldToScreenPoint(roomPos);

            // calculate rect for rendering label
            mTmpRect.x = hotSpotPos.x - cHotspotSizeX / 2;
            mTmpRect.y = Screen.height - hotSpotPos.y - cHotspotSizeY / 2;
            mTmpRect.width = cHotspotSizeX;
            mTmpRect.height = cHotspotSizeY;

            // now render label at this point
            GUI.Box(mTmpRect, mGORoomsNames[i]);
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    private void SelectObjectByMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Constants.cMaxRayCastDistance))
        {
            // get game object
            GameObject rayCastedGO = hit.collider.gameObject;

            // select object
            this.SelectedObject = rayCastedGO;
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    /// <summary>
    /// Gets or sets selected GameObject
    /// </summary>
    public GameObject SelectedObject
    {
        get
        {
            return mSelectedObject;
        }
        set
        {
            // get old game object
            GameObject goOld = mSelectedObject;

            // assign new game object
            mSelectedObject = value;

            // if this object is the same - just not process this
            if (goOld == mSelectedObject)
            {
                return;
            }

            // set material to non-selected object
            if (goOld != null)
            {
                goOld.renderer.material = SimpleMat;
            }

            // set material to selected object
            if (mSelectedObject != null)
            {
                mSelectedObject.renderer.material = HighlightedMat;
            }
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
