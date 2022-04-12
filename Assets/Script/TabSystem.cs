using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSystem : MonoBehaviour
{
    public Transform TabArea;
    public Transform PageArea;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void BtnClick(int idx)
    {
        // Debug.Log(idx);
        for (int i=0;i<5;i++){
            // Debug.Log(PageArea.GetChild(i).gameObject);
            if (i == idx){
                PageArea.GetChild(i).gameObject.SetActive(true);
            }else{
                PageArea.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
