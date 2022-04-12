using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_LIne_Renderer : MonoBehaviour
{
    public Transform line_prefab;
    // Start is called before the first frame update
    void Start()
    {
        
        for(int i=0;i<40;i++){
            float x = i * 0.2f + 118.5f;
            for(int j=0;j<23;j++){
                float y = j * -0.2f + 27.5f;

                var newLine = Instantiate (line_prefab, Vector3.zero , Quaternion.identity);
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
