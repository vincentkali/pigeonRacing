using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PigeonCamera : MonoBehaviour
{
    [Header("---- upper panel ----")]
    public int targetId;
    public int pigeonNumber;
    public Button decreaseButton;
    public Button IncreaseButton;
    public InputField mainInputField;
    public Transform pigeon_prefab;
    public GameObject pigeons;
    public static GameObject[] targetArray;
    [Header("---- middle upper panel ----")]
    public Vector3 offset;

    public Button ZoomInButton;
    public Button ZoomOutButton;
    [Header("---- middle lower panel ----")]
    public float miniMapOffsetRatio;
    public float pointSizeAdjust;
    private float sizeLat;
    private float sizeLng;
    private float size;
    public Material currentLookingMtl;
    public Material notLookingMtl;
    private int originTargetId;
    private float scale;
    [Header("---- 3D canvas ----")]
    public RectTransform startingUI;
    public RectTransform endingUI;


    void Start()
    {
        originTargetId = targetId; // Init Camera Target

        // AddListener
        ZoomOutButton.onClick.AddListener(ZoomOutButtonOnClick);
        ZoomInButton.onClick.AddListener(ZoomInButtonOnClick);
        decreaseButton.onClick.AddListener(decreaseButtonOnClick);
        IncreaseButton.onClick.AddListener(IncreaseButtonOnClick);
        mainInputField.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        

        miniMapInit();

        // Instantiate Pigeons
        pigeonNumber = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().number;
        // Debug.Log("pigeonAttributes.number: " + pigeonNumber);
        // pigeonNumber = 4;
        targetArray = new GameObject[pigeonNumber+1];
        for (int i = 1; i < pigeonNumber+1; i++)
        {
            var newPigeon = Instantiate (pigeon_prefab, new Vector3(-1 * (i-1),0,0) , Quaternion.identity);
            newPigeon.transform.parent = pigeons.transform; // Put it under Pigeons Gameobject

            // Init Path and target
            newPigeon.GetComponent<Pigeon>().pigeonId = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeons[i-1].pigeonId + 1;
            newPigeon.GetComponent<Pigeon>().nPath = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeons[i-1].nPath;
            newPigeon.GetComponent<Pigeon>().path = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeons[i-1].path;
            newPigeon.transform.Find("mini map marker").transform.localScale *= scale;
            targetArray[i] = newPigeon.gameObject.transform.Find("target").gameObject;
        }
        // keyin all the path

        // Init the First Target Marker to be Current Look Marker
        offset = new Vector3(1.20f,0.24f,0.67f);
        targetArray[1].transform.parent.gameObject.transform.Find("mini map marker").gameObject.GetComponent<MeshRenderer>().material = currentLookingMtl;
        targetArray[1].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.localScale *= 2;
        targetArray[1].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.position += new Vector3(0,10,0);

        
    }
    void miniMapInit()
    {
        // mini map camera center
        float miniMapX = MapToGameX(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lng);
        miniMapX += MapToGameX(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lng);
        miniMapX /= 2;     
        
        float miniMapY = MapToGameY(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lat);
        miniMapY += MapToGameY(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lat);
        miniMapY /= 2;     

        float originHeight = GameObject.Find("mini map Camera").transform.position.y;
        GameObject.Find("mini map Camera").transform.position = new Vector3(miniMapX, originHeight, miniMapY);

        // mini map camera size
        sizeLat = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lat;
        sizeLat -= GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lat;
        sizeLat = Mathf.Abs(sizeLat) * 200 * 1/2 * miniMapOffsetRatio;

        sizeLng = GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lng;
        sizeLng -= GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lng;
        sizeLng = Mathf.Abs(sizeLng) * 200 * 1/2 * miniMapOffsetRatio;

        size = Mathf.Max(sizeLat, sizeLng);

        GameObject.Find("mini map Camera").GetComponent<Camera>().orthographicSize = size;

        // starting/ending point position
        float startingX = MapToGameX(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lng);
        float startingY = MapToGameY(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.starting_point_lat);
        float endingX = MapToGameX(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lng);
        float endingY = MapToGameY(GameObject.Find("ParameterControl").GetComponent<ParameterControl>().pigeonAttributes.ending_point_lat);

        originHeight = GameObject.Find("starting point").transform.position.y;
        // Debug.Log("startingX: " + startingX);
        // Debug.Log("startingY: " + startingY);
        // Debug.Log("endingX: " + endingX);
        // Debug.Log("endingY: " + endingY);

        startingUI.anchoredPosition = new Vector3(startingX,startingY,-90);
        endingUI.anchoredPosition = new Vector3(endingX,endingY,-90);

        GameObject.Find("starting point").transform.position = new Vector3(startingX, originHeight, startingY);
        originHeight = GameObject.Find("ending point").transform.position.y;
        GameObject.Find("ending point").transform.position = new Vector3(endingX, originHeight, endingY);

        // starting/ending point scale
        scale = (size/300) * pointSizeAdjust;
        startingUI.localScale *= scale;
        endingUI.localScale *= scale;
        
        GameObject.Find("starting point").transform.localScale *= scale;
        GameObject.Find("ending point").transform.localScale *= scale;

        // Debug.Log("size: " + size + "  scale: " + scale);

        // mini map dots scale

        // unfinish

    }
    void ZoomOutButtonOnClick(){
        Scale(-1);
	}
    void ZoomInButtonOnClick(){
        Scale(1);
	}

    void LateUpdate()
    {
        if (pigeonNumber > 0)
        {
            transform.position = targetArray[targetId].transform.position + offset;
            Rotate();
        }
    }
    void Update()
    {
        
        
    }
    private void Scale(int control)
    {
        float dis = offset.magnitude;
        float c = control;
        dis -= c * 1f;
        if (dis < 1 || dis > 20)
        {
            return;
        }
        offset = offset.normalized * dis;
    }
    private void Rotate()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = transform.position;
            Vector3 rot = transform.eulerAngles;

            transform.RotateAround(targetArray[targetId].transform.position, Vector3.up, Input.GetAxis("Mouse X") * 10);
            float y = transform.eulerAngles.y;
            offset = transform.position - targetArray[targetId].transform.position;
        }

    }
    void decreaseButtonOnClick()
    {
        if (targetId >= 2)
        {
            targetId -= 1;
        } 
        
        mainInputField.text = targetId.ToString();
        // Debug.Log(targetId);
    }
    void IncreaseButtonOnClick()
    {
        if (targetId <= pigeonNumber - 1)
        {
            targetId += 1;
        }
        mainInputField.text = targetId.ToString();
        // Debug.Log(targetId);
    }
    void ValueChangeCheck()
    {
        // Debug.Log("Value Changed");
        if (mainInputField.text.Length != 0)
        {
            int temp = int.Parse(mainInputField.text);
            if (temp >= 1 && temp <= pigeonNumber)
            {
                targetId = temp;
                targetArray[originTargetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.GetComponent<MeshRenderer>().material = notLookingMtl;
                targetArray[originTargetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.localScale /= 2;
                targetArray[originTargetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.position -= new Vector3(0,10,0);

                targetArray[targetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.GetComponent<MeshRenderer>().material = currentLookingMtl;
                targetArray[targetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.localScale *= 2;
                targetArray[targetId].transform.parent.gameObject.transform.Find("mini map marker").gameObject.transform.position += new Vector3(0,10,0);

                originTargetId = targetId;
            }
        }
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