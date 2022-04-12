using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Transform buttons;
    private Button btn;
    void Start()
    {
        btn = buttons.GetChild(0).GetComponent<Button>();
        btn.Select();
    }
    void Update()
    {
        
    }
    public void marketBtnClick()
    {

    }
    public void socialBtnClick()
    {
        
    }
    public void cageBtnClick()
    {
        
    }
    public void raceBtnClick()
    {
        
    }
    public void rankBtnClick()
    {
        
    }
    
}
