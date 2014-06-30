using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppRoot : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    #region Variables

    private static AppRoot mInstance;

    // 
    private const float cRecordTimePeriod = 0.001f;
    
    private float mCurrentTime = 0.0f;
    
    // saving gos state data
    private List<Dictionary<GameObject, SavedState>> mSavedStates = new List<Dictionary<GameObject, SavedState>>();
    private Dictionary<GameObject, SavedState> mFirstState = new Dictionary<GameObject, SavedState>();
    private int mCurrentStateIndex = -1;
    private int mCurrSliderValue = 0;

    // current game state
    private EGameState mGameState = EGameState.None;

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Interface

    /// <summary>
    /// ctor
    /// </summary>
    public AppRoot()
    {
        mInstance = this;
    }
    
    public void Start()
    {
        // this will allow us to restore application state
        SaveGosFirstState();
    }
    
    public void Update()
    {
        // saves states
        if (mGameState == EGameState.Recording)
        {
            mCurrentTime += Time.deltaTime;

            if (mCurrentTime >= cRecordTimePeriod)
            {
                SaveGosCurrentState();

                // reset current time
                mCurrentTime = 0.0f;
            }
        }
    }

    public void OnGUI()
    {
        // depending on state application will show separate ui
        switch (mGameState)
        {
            case EGameState.None:
                {
                    if (GUI.Button(new Rect(100, 100, 200, 40), "RECORD"))
                    {
                        // remove old data
                        mSavedStates.Clear();
                        mCurrentStateIndex = -1;
                        mCurrSliderValue = 0;

                        mGameState = EGameState.Recording;
                    }
                    break;
                }
            case EGameState.Recording:
                {
                    if (GUI.Button(new Rect(100, 100, 200, 40), "Recording... STOP!"))
                    {
                        mGameState = EGameState.Reproduce;

                        // disable physics 
                        EnablePhysics(false);
                    }

                    break;
                }
            case EGameState.Reproduce:
                {
                    // draw slider with time frames
                    mCurrSliderValue = (int)GUI.HorizontalSlider(new Rect(100, 150, 300, 30), mCurrSliderValue, 0, mSavedStates.Count - 1);
                    GUI.Label(new Rect(100, 180, 100, 40), "State num: " + mCurrSliderValue);

                    // setting the state that is selected on slider
                    SetGosState(mCurrSliderValue);

                    // if user want to reset application state, do it
                    if (GUI.Button(new Rect(100, 100, 200, 40), "Restore GOS"))
                    {
                        //
                        SetGosStateByDict(mFirstState);

                        // enable physics
                        EnablePhysics(true);

                        //
                        mCurrentStateIndex = -1;
                        mCurrSliderValue = 0;

                        mGameState = EGameState.None;
                    }

                    break;
                }
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Implementation

    /// <summary>
    /// Just set "Kinematic" for all game objects
    /// </summary>
    /// <param name="enable"></param>
    private void EnablePhysics(bool enable)
    {
        Rigidbody[] rigid = GameObject.FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody r in rigid)
        {
            r.isKinematic = !enable;
        }
    }

    /// <summary>
    /// Saves current time state of the scene
    /// </summary>
    private void SaveGosCurrentState()
    {
        if (mGameState != EGameState.Recording)
        {
            return;
        }

        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();

        // create current time state
        Dictionary<GameObject, SavedState> stateData = new Dictionary<GameObject, SavedState>();

        // add game objects to dict
        foreach (GameObject go in gos)
        {
            SavedState state = new SavedState() 
            { 
                GO = go,
                Position = go.transform.position,
                Rotation = go.transform.rotation,
                ScaleLocal = go.transform.localScale,
                Time = Time.realtimeSinceStartup
            };
            stateData[go] = state;
        }

        // add current time state to list
        mSavedStates.Add(stateData);
    }

    /// <summary>
    /// Save this first game state, it will allow us to revert scene state
    /// </summary>
    private void SaveGosFirstState()
    {
        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();

        // add game objects to dict
        foreach (GameObject go in gos)
        {
            SavedState state = new SavedState()
            {
                GO = go,
                Position = go.transform.position,
                Rotation = go.transform.rotation,
                ScaleLocal = go.transform.localScale,
                Time = Time.realtimeSinceStartup
            };
            mFirstState[go] = state;
        }

    }

    /// <summary>
    /// Sets application state by state index
    /// </summary>
    /// <param name="stateIndex"></param>
    private void SetGosState(int stateIndex)
    {
        if (stateIndex >= mSavedStates.Count)
        {
            Debug.LogWarning("Error! Cannot set state: " + stateIndex + ". Index out of range");
            return;
        }

        if (mCurrentStateIndex == stateIndex)
        {
            return;
        }

        Dictionary<GameObject, SavedState> currentStateData = mSavedStates[stateIndex];

        SetGosStateByDict(currentStateData);

        mCurrentStateIndex = stateIndex;
    }

    private void SetGosStateByDict(Dictionary<GameObject, SavedState> currentStateData)
    {
        // restore time data
        foreach (KeyValuePair<GameObject, SavedState> kvp in currentStateData)
        {
            SavedState savedData = kvp.Value;
            GameObject goToRestore = savedData.GO;

            // restore transformations
            goToRestore.transform.position = savedData.Position;
            goToRestore.transform.rotation = savedData.Rotation;
            goToRestore.transform.localScale = savedData.ScaleLocal;
        }
    }

    #endregion
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region Properties

    public static AppRoot Instance
    {
        get
        {
            return mInstance;
        }
    }

    #endregion    
    ///////////////////////////////////////////////////////////////////////////
}

/// <summary>
/// This is data stored for one frame of the saved data
/// </summary>
public class SavedState
{
    public GameObject GO;
    public float Time;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 ScaleLocal;
}

/// <summary>
/// Possible application states
/// </summary>
public enum EGameState
{
    None = 0,
    Recording,
    Reproduce
}
