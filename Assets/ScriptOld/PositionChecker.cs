using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionChecker : MonoBehaviour
{
    // Start is called before the first frame update
    public float E;
    public float N;

    void Start()
    {
        N = 26.43f;
        E = 122.27f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(MapToGameX(E), 0, MapToGameY(N));
    }
    float MapToGameX(float E)
    {
        return (E - 120.75f) * 200;
    }
    float MapToGameY(float N)
    {
        return (N - 24.0f) * 200;
    }
}
