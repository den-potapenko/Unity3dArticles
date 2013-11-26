using UnityEngine;
using System.Collections;

public class AppRoot : MonoBehaviour
{
    private TransformObject mTransform; // TransformObject implements rotate / pan / zoom
    
    private GameObject mGOFlat; // GO rotate around
    private const string cGONameFlat = "Flat"; 

    void Start()
    {
		// Find cGONameFlat in scene
        mGOFlat = GameObject.Find(cGONameFlat);

		// instantiate TransformObject and sets its rotate around object
        mTransform = new TransformObject();
        mTransform.SetTransformRotateAround(mGOFlat.transform);
    }

    void Update()
    {
        mTransform.Update();
    }
}
