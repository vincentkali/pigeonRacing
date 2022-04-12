using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line_test : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer LR;
    public float direction;
    public float length;
    public float x;
    public float y;
    void Start()
    {
        direction = 0;
        length = 1;
    }

    // Update is called once per frame
    void Update()
    {
        PolarToCartesian(direction, length,out x,out y);
        LR.SetPosition(1, new Vector3(x, 0, y));
    }
    void PolarToCartesian(float direction, float length,out float x, out float y)
    {
        float radians = direction * Mathf.Deg2Rad; 
        x = Mathf.Sin(radians) * length;
        y = Mathf.Cos(radians) * length;
    }
}
