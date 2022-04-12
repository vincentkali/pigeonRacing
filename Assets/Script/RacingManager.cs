using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacingManager : MonoBehaviour
{
    MyClient myclient;
    public GameObject GameInfoGoto;
    public GameObject tabArea;
    public GameObject racingPage;
    public GameObject loftPage;
    public Transform pigeons;
    public GameObject racingCheck;
    public Transform gameInfos;
    public Sprite waitGameSpt;
    public bool selectPigeon = false;
    long[] idpigeon;
    List<int> selectedPigeons = new List<int>();
    long sn;
    int npigeon;
    int gameIdx;

    /// 
    string gameIdxText;
    string gameNameText;
    string gamePrizeText;
    string gameRequireText;
    string startPointText;
    string startPointCoefText;
    string distanceText;
    string daysText;
    string conditionText;
    string typeText;
    string timeText;
    string feeText;
    int pigeonRe;
    int pigeonHas;

    void Start()
    {
        myclient = StaticObject.myclient;
    }
    public void RacingBtnClick(int idx)
    {
        gameIdx = idx;
        // Debug.Log($"idx: {idx}");
        racingPage.SetActive(false);
        loftPage.SetActive(true);
        selectPigeon = true;

        int N = pigeons.childCount;
        for(int i=0;i<N;i++){
            pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").gameObject.SetActive(true);
        }
        racingCheck.SetActive(true);


        GameObject selectGameInfo = gameInfos.GetChild(idx).gameObject;
        string sn = GameInfoGoto.transform.Find("GameIdx").GetComponent<Text>().text = selectGameInfo.transform.Find("GameIdx").GetComponent<Text>().text;
        long snLong = long.Parse(sn);
        pigeonRe = StaticObject.sn2require[snLong];
        GameInfoGoto.transform.Find("GameName").GetComponent<Text>().text = selectGameInfo.transform.Find("GameName").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("GamePrize").GetComponent<Text>().text = selectGameInfo.transform.Find("GamePrize").GetComponent<Text>().text;
        string re = GameInfoGoto.transform.Find("PigeonRequire").GetComponent<Text>().text = selectGameInfo.transform.Find("PigeonRequire").GetComponent<Text>().text;
        string[] subs = re.Split('/');
        pigeonRe = int.Parse(subs[1]);
        pigeonHas = int.Parse(subs[0]);
        Debug.Log($"pigeonRe: {pigeonRe}");
        Debug.Log($"pigeonHas: {pigeonHas}");
        
        GameInfoGoto.transform.Find("StartPoint").GetComponent<Text>().text = selectGameInfo.transform.Find("StartPoint").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("StartPointCoef").GetComponent<Text>().text = selectGameInfo.transform.Find("StartPointCoef").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Distance").GetComponent<Text>().text = selectGameInfo.transform.Find("Distance").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Days").GetComponent<Text>().text = selectGameInfo.transform.Find("Days").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Condition").GetComponent<Text>().text = selectGameInfo.transform.Find("Condition").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Type").GetComponent<Text>().text = selectGameInfo.transform.Find("Type").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Fee").GetComponent<Text>().text = selectGameInfo.transform.Find("Fee").GetComponent<Text>().text;
        GameInfoGoto.transform.Find("Time").GetComponent<Text>().text = selectGameInfo.transform.Find("SignUpBtn").Find("Time").GetComponent<Text>().text;
        // var newObject = Instantiate(StaticObject.selectGameInfo, new Vector3(0, 0, 0), Quaternion.identity);
        // newObject.transform.parent = racingCheck.transform;
        // newObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(280, -123, 0);
        // newObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f,1,1);
        // newObject.transform.Find("FeeText").gameObject.SetActive(false);
        // newObject.transform.Find("Fee").gameObject.SetActive(false);
        // newObject.transform.Find("SignUpBtn").gameObject.SetActive(false);

        tabArea.transform.Find("Loft").gameObject.GetComponent<Button>().interactable = false;
        tabArea.transform.Find("Racing").gameObject.GetComponent<Button>().interactable = false;
        tabArea.transform.Find("Ranking").gameObject.GetComponent<Button>().interactable = false;
        tabArea.transform.Find("Social").gameObject.GetComponent<Button>().interactable = false;
        tabArea.transform.Find("Market").gameObject.GetComponent<Button>().interactable = false;
    }
    public void RacingCancelClick()
    {
        int N = pigeons.childCount;
        for(int i=0;i<N;i++){
            pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").gameObject.SetActive(false);
        }
        racingCheck.SetActive(false);
        racingPage.SetActive(true);
        loftPage.SetActive(false);

        tabArea.transform.Find("Loft").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Racing").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Ranking").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Social").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Market").gameObject.GetComponent<Button>().interactable = true;
    }

    public void RacingCheckClick()
    {
        selectedPigeons.Clear();
        int selectedNumber=0;
        int N = myclient.S2C_PLAYER_PIGEON_obj.npigeon;
        List<long> pigeonIdList = new List<long>();
        for(int i=0;i<N;i++){
            if (pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").GetComponent<Toggle>().isOn){
                pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").GetComponent<Toggle>().isOn = false;
                selectedNumber++;
                pigeonIdList.Add(pigeons.GetChild(i).Find("Pigeon display").GetComponent<PigeonDisplay>().pigeonId);
                selectedPigeons.Add(i);
                Debug.Log(pigeons.GetChild(i).Find("name").GetComponent<Text>().text);
                pigeons.GetChild(i).Find("ToggleOut").gameObject.SetActive(false);
            }   
        }
        if (selectedNumber == 0){
            racingCheck.SetActive(false);
            racingPage.SetActive(true);
            loftPage.SetActive(false);
            for(int i=0;i<N;i++){
                pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").gameObject.SetActive(false);
            }
            return;
        }


        sn = long.Parse(gameInfos.GetChild(gameIdx).Find("GameIdx").GetComponent<Text>().text);
        gameInfos.GetChild(gameIdx).Find("SignUpBtn").GetComponent<Image>().sprite = waitGameSpt;
        // gameInfos.GetChild(gameIdx).Find("SignUpBtn").GetComponent<Button>().interactable = false;
        gameInfos.GetChild(gameIdx).Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "等待比賽";
        
        if (selectedNumber + pigeonHas >= pigeonRe){
            gameInfos.GetChild(gameIdx).Find("SignUpBtn").Find("Mask").gameObject.SetActive(true);
            gameInfos.GetChild(gameIdx).Find("SignUpBtn").Find("Time").gameObject.SetActive(true);
        }else {
            gameInfos.GetChild(gameIdx).Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
            gameInfos.GetChild(gameIdx).Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
        }
        

        npigeon = selectedNumber;
        idpigeon = pigeonIdList.ToArray();
        for(int i=0;i<idpigeon.Length;i++){
            Debug.Log($"send idpigeon: {idpigeon[i]}");
        }
        Debug.Log($"send sn: {sn}");
        selectPigeon = false;
        myclient.canUpdate = false;
        myclient.C2S_PIGEON_ACTION(PlayerPrefs.GetInt("pid"), PlayerPrefs.GetInt("rkey"), sn, idpigeon, npigeon);
        StartCoroutine(waitForComplete1());

        // foreach(long e in idpigeon){
        //     Debug.Log($"id: {e}");
        // }

        racingCheck.SetActive(false);
        racingPage.SetActive(true);
        loftPage.SetActive(false);
        N = pigeons.childCount;
        for(int i=0;i<N;i++){
            pigeons.GetChild(i).Find("ToggleOut").Find("Toggle").gameObject.SetActive(false);
        }

        tabArea.transform.Find("Loft").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Racing").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Ranking").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Social").gameObject.GetComponent<Button>().interactable = true;
        tabArea.transform.Find("Market").gameObject.GetComponent<Button>().interactable = true;
    }
    IEnumerator waitForComplete1()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;
        myclient.canUpdate = true;
        // Debug.Log("can update");

        Debug.Log($"action : {myclient.S2C_PIGEON_ACTION_obj.action}");
        // for (int i=0; i<npigeon; i++){ 
        //     Debug.Log($"idpigeon [{i}]: {myclient.S2C_PIGEON_ACTION_obj.idpigeon[i]}");
        // }

    }
}
