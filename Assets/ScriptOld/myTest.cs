using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myTest : MonoBehaviour
{
    public GameObject target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, Time.deltaTime * speed);
    }
}
