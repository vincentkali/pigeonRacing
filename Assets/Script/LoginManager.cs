using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text Name_input;
    public Text ErrorMsg;
    MyClient myclient;
    public Transform NoName;
    public Transform HasName;
    
    void Start()
    {
        myclient = StaticObject.myclient;
        if (PlayerPrefs.HasKey("hasname")){
            NoName.gameObject.SetActive(false);
            HasName.gameObject.SetActive(true);
            HasName.Find("welcomeText").GetComponent<Text>().text = "歡迎 " + PlayerPrefs.GetString("name");
        }
        ErrorMsg.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HasName_StartGame_btn_OnClick()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void NoName_StartGame_btn_OnClick()
    {
        if (Name_input.text == ""){
            ErrorMsg.text = "請輸入名稱";
            return;
        }
        if (Name_input.text.Length > 9){
            ErrorMsg.text = "名稱最多九個字";
            return;
        }
        if (checkName(Name_input.text) == false){
            ErrorMsg.text = "名稱僅可含中文、英文、數字";
            return;
        }
        ErrorMsg.text = "";
        
        string name = Name_input.text;
        string token = tokenGenerator();
   
        PlayerPrefs.SetString("token", token);
        PlayerPrefs.SetString("name", name);
        PlayerPrefs.SetInt("hasName", 1);

        myclient.C2S_LOGIN_PLAY(token, name);
        Debug.Log($"Send Token: {token}");
        Debug.Log($"Send Name: {name}");
        StartCoroutine(waitForComplete());
        
    }
    IEnumerator waitForComplete()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }

        myclient.complete = false;
        PlayerPrefs.SetInt("pid", myclient.S2C_LOGIN_obj.pid);
        PlayerPrefs.SetInt("rkey", myclient.S2C_LOGIN_obj.rkey);

        byte[] buffer = myclient.S2C_LOGIN_obj.raw;
        Debug.Log("recvData: ");
        myclient.printByte(buffer);

        
        SceneManager.LoadScene("Lobby");
    }
    public string tokenGenerator()
    {
        string token = "";
        string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i=0;i<10;i++){
            int index = Random.Range(0, 26+26+10);
            token += alphabet[index];
        }
        return token;
    }
    public bool checkName(string s_)
    {
        for (int i=0; i<s_.Length; i++){
            if (!(s_[i] >= 0x4E00 && s_[i] <= 0x9FA5) 
                && !(s_[i] >= '0'&& s_[i] <= '9') 
                && !(s_[i] >= 'a' && s_[i] <= 'z')){
                return false;
            }
        }
        return true;
    }
    void OnApplicationQuit()
    {
        myclient.serverSocket.Close();
        Debug.Log("quit");
    }
}
