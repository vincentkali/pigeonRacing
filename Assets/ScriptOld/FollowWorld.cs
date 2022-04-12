using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorld : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Transform lookAt;
    [SerializeField] public Vector3 offset;
    public Camera cam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);

        if (transform.position != pos)
            transform.position = pos;
    }
}
