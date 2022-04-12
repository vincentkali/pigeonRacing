using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myTest2 : MonoBehaviour
{
    // Start is called before the first frame update
    public float lng;
    public float lat;
    void Start()
    {
        lng = 118.5f;
        lat = 27.5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(MapToGameX(lng), 40, MapToGameY(lat));
    }
    float MapToGameX(float E)
    {
        // origin: 120.75 -> 0
        // size: 4.5 -> 450 
        return (E - 120.75f) * 200;
    }
    float MapToGameY(float N)
    {
        // origin: 24 -> 0
        // size: 7 -> 700
        return (N - 24.0f) * 200;
    }
}
