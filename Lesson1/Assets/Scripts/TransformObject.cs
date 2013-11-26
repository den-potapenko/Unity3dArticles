using UnityEngine;
using System;

public class TransformObject
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    // variables

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER
    private float RotationSpeed = 1500;
    private float MoveSpeed = 10.0f;
    private float ZoomSpeed = 15.3f;
#endif // UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBPLAYER

#if UNITY_ANDROID || UNITY_IPHONE
    private float RotationSpeed = 9.5f;
    private float MoveSpeed = 1.09f;
    private float ZoomSpeed = 0.009f;

    private float mOldFingerDelta = 0;
    private const float mFingerDistanceEpsilon = 1.0f;

#endif // UNITY_ANDROID || UNITY_IPHONE

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

#if UNITY_ANDROID || UNITY_IPHONE

        // rotate
        if (Input.touchCount == 1)
        {
            mMoveObject.Rotate(camLeft, Input.touches[0].deltaPosition.y * RotationSpeed * Time.deltaTime, Space.World);
            mMoveObject.Rotate(Vector3.down, Input.touches[0].deltaPosition.x * RotationSpeed * Time.deltaTime, Space.Self);
        }

        if (Input.touchCount == 2)
        {
            UnityEngine.Vector2 deltaFingerVec = Input.touches[0].position - Input.touches[1].position;

            float deltaFingerValue =
                Mathf.Sqrt(deltaFingerVec.x * deltaFingerVec.x + deltaFingerVec.y * deltaFingerVec.y);

            // move
            bool moved = false;
            {
                float moveX = 0;
                float moveY = 0;

                // moveX
                if (Input.touches[0].deltaPosition.x < 0 && Input.touches[1].deltaPosition.x < 0)
                {
                    moveX = Mathf.Max(Input.touches[0].deltaPosition.x, Input.touches[1].deltaPosition.x);
                    moved = true;
                }
                else if (Input.touches[0].deltaPosition.x > 0 && Input.touches[1].deltaPosition.x > 0)
                {
                    moveX = Mathf.Min(Input.touches[0].deltaPosition.x, Input.touches[1].deltaPosition.x);
                    moved = true;
                }

                // moveY 
                if (Input.touches[0].deltaPosition.y < 0 && Input.touches[1].deltaPosition.y < 0)
                {
                    moveY = Mathf.Max(Input.touches[0].deltaPosition.y, Input.touches[1].deltaPosition.y);
                    moved = true;
                }
                else if (Input.touches[0].deltaPosition.y > 0 && Input.touches[1].deltaPosition.y > 0)
                {
                    moveY = Mathf.Min(Input.touches[0].deltaPosition.y, Input.touches[1].deltaPosition.y);
                    moved = true;
                }

                Vector3 camPos = Camera.main.transform.position;
                camPos += -camLeft * MoveSpeed * moveX * Time.deltaTime;
                camPos += -camDown * MoveSpeed * moveY * Time.deltaTime;
                Camera.main.transform.position = camPos;
            }

            // zoom
            if (!moved && Mathf.Abs(deltaFingerValue - mOldFingerDelta) > mFingerDistanceEpsilon)
            {
                if (deltaFingerValue - mOldFingerDelta > 0)
                {
                    if (dist > MinDist)
                    {
                        mMoveObject.transform.Translate(-dir * ZoomSpeed * Time.deltaTime * deltaFingerValue, Space.World);
                    }
                }

                if (deltaFingerValue - mOldFingerDelta < 0)
                {
                    if (dist < MaxDist)
                    {
                        mMoveObject.transform.Translate(dir * ZoomSpeed * Time.deltaTime * deltaFingerValue, Space.World);
                    }
                }
            }

            mOldFingerDelta = deltaFingerValue;
        }


#endif // UNITY_ANDROID || UNITY_IPHONE



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
