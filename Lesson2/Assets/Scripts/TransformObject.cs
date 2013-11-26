using UnityEngine;
using System;

public class TransformObject
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    // variables

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER
    private float RotationSpeed = 1500;
    private float MoveSpeed = 50.0f;
    private float ZoomSpeed = 15.3f;
#endif // UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER

    public float MinDist = 2.0f;
	public float MaxDist = 50.0f;

    private Transform mMoveObject = null;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Public methods

	/// <summary>
	/// 
	/// </summary>
    public TransformObject()
    {
        EnabledMoving = true;
    }

	/// <summary>
	/// Sets transform that will be used as "center" of the rotate / pan / zoom
	/// </summary>
    public void SetTransformRotateAround(Transform goMove)
    {
        mMoveObject = goMove;
        if (mMoveObject == null)
        {
            Debug.LogWarning("Error! Cannot find object!");
            return;
        }
    }

    public void Update()
    {
        if (!EnabledMoving)
        {
            return;
        }

        Vector3 dir = mMoveObject.position - Camera.main.transform.position;
        float dist = Math.Abs(dir.magnitude);

        Vector3 camDir = Camera.main.transform.forward;
        Vector3 camLeft = Vector3.Cross(camDir, Vector3.down);
        Vector3 camDown = Vector3.Cross(camDir, camLeft);
        //Vector3 camUp = Vector3.Cross(camLeft, camDir);

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        // rotate
        if (Input.GetMouseButton(0))
        {
            mMoveObject.Rotate(camLeft, dy * RotationSpeed * Time.deltaTime, Space.World);
            mMoveObject.Rotate(Vector3.down, dx * RotationSpeed * Time.deltaTime, Space.Self);
        }

        // move
        if (Input.GetMouseButton(1))
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos += -camLeft * MoveSpeed * dx * Time.deltaTime;
            camPos += -camDown * MoveSpeed * dy * Time.deltaTime;
            Camera.main.transform.position = camPos;
        }

        // zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (dist > MinDist)
            {
                mMoveObject.Translate(-dir * ZoomSpeed * Time.deltaTime, Space.World);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (dist < MaxDist)
            {
                mMoveObject.Translate(dir * ZoomSpeed * Time.deltaTime, Space.World);
            }
        }

#endif // UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties
    /// <summary>
    /// Gets or set value indicating if transformation is enabled
    /// </summary>
    public bool EnabledMoving
    {
        get;
        set;
    }

    /// <summary>
    /// Gets game object that moves around
    /// </summary>
    public Transform MoveObject
    {
        get
        {
            return mMoveObject;
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////
}
