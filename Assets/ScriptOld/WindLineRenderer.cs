using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindLineRenderer : MonoBehaviour
{
    public Transform line_prefab;
    public GameObject Lines;
    public float LengthAdjust;
    public float minLenCanBeDraw;
    public float drawLinePercent;
    float dx;
    float dy;
    // Start is called before the first frame update
    void Start()
    {
        float miniMapOffsetRatio = 1.4f;
        float pointSizeAdjust = 1;
        float sizeLat = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lat;
        sizeLat -= GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lat;
        sizeLat = Mathf.Abs(sizeLat) * 200 * 1/2 * miniMapOffsetRatio;

        float sizeLng = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lng;
        sizeLng -= GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lng;
        sizeLng = Mathf.Abs(sizeLng) * 200 * 1/2 * miniMapOffsetRatio;

        float size = Mathf.Max(sizeLat, sizeLng);
        float scale = (size/300) * pointSizeAdjust;

        float direction = 90;
        float length = 50;
        
        for(int i=0;i<23;i++){
            float x = i * 0.2f + 118.5f;
            for(int j=0;j<40;j++){
                float y = j * -0.2f + 27.5f;

                direction = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().wind_direction[i][j];
                length = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().wind_speed[i][j] * LengthAdjust;
                // Debug.Log("wind_direction (" + i + " , " + j + " ): " + direction);
                // Debug.Log("wind_speed (" + i + " , " + j + " ): " + length);

                if (length < minLenCanBeDraw) continue;
                if (drawLinePercent <= Random.Range(0, 100)) continue;

                var newLine = Instantiate (line_prefab, Vector3.zero , Quaternion.identity);
                newLine.transform.parent = Lines.transform; // Put it under Pigeons Gameobject

                PolarToCartesian(direction, length,out dx, out dy);
                newLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(MapToGameX(x), 90, MapToGameY(y)));
                newLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(MapToGameX(x) + dx * 0.5f, 90, MapToGameY(y) + dy * 0.5f));
                newLine.GetComponent<LineRenderer>().SetPosition(2, new Vector3(MapToGameX(x) + dx * 1, 90, MapToGameY(y) + dy * 1));
                newLine.GetComponent<LineRenderer>().SetPosition(3, new Vector3(MapToGameX(x) + dx * 1.5f, 90, MapToGameY(y) + dy * 1.5f));
                
                newLine.GetComponent<LineRenderer>().startWidth *= scale;
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
    void PolarToCartesian(float direction, float length,out float dx, out float dy)
    {
        float radians = direction * Mathf.Deg2Rad; 
        dx = Mathf.Sin(radians) * length;
        dy = Mathf.Cos(radians) * length;
    }
}
