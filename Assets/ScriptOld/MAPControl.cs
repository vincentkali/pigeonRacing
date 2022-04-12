using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAPControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 startingPointInput;
    public Vector2 endingPointInput;
    public Vector3 startingPoint3D;
    public Vector3 endingPoint3D;
    void Start()
    {
        startingPointInput = new Vector2(26.43f, 122.27f);
        endingPointInput = new Vector2(24.16f, 120.65f);
        // from parameter get startingPoint, endingPoint
        // starting point toScreen
    }

    // Update is called once per frame
    void Update()
    {
        // PigeonCamera.targetArray
    }
}
