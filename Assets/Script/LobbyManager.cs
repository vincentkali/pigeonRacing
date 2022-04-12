using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text nickname_text;
    MyClient myclient;
    Dictionary<long, int> sn2idx;
    Dictionary<long, byte> sn2status;
    Dictionary<long, int> sn2require;
    OpenGameData[] openGameDatas;
    GameObject selectGameInfo;
    public Transform pigeons;
    public Transform GameInfos;
    public Sprite RegisterSp;
    public Sprite GamingSp;
    public Sprite GameOverSp;
    public Sprite WaitGameSp;

    void Start()
    {
        myclient = StaticObject.myclient;
        openGameDatas = StaticObject.openGameDatas;
        sn2status = StaticObject.sn2status;
        sn2idx = StaticObject.sn2idx;
        sn2require = StaticObject.sn2require;
        string nickname = PlayerPrefs.GetString("name");
        nickname_text.text = nickname;
        myclient.C2S_PLAYER_PIGEON(PlayerPrefs.GetInt("pid"), PlayerPrefs.GetInt("rkey"));
        StartCoroutine(waitForComplete1());
        
    }
    

    IEnumerator waitForComplete1()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }

        myclient.complete = false;

        int npigeon = myclient.S2C_PLAYER_PIGEON_obj.npigeon;
        // Debug.Log($"npigeon: {npigeon}");
        // npigeon = Mathf.Min(12, npigeon);
        for (int i=0;i<npigeon;i++){
            pigeons.GetChild(i).gameObject.SetActive(true);
            pigeons.GetChild(i).Find("name").GetComponent<Text>().text = myclient.S2C_PLAYER_PIGEON_obj.pigeonName_list[i];
            pigeons.GetChild(i).Find("Pigeon display").GetComponent<PigeonDisplay>().pigeonId = myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].id;
        }
        

        byte[] buffer = myclient.S2C_LOGIN_obj.raw;
        // Debug.Log("recvData: ");
        // myclient.printByte(buffer);
        myclient.C2S_GAME_LIST(PlayerPrefs.GetInt("pid"), PlayerPrefs.GetInt("rkey"));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(waitForComplete2());
    }
    IEnumerator waitForComplete2()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }

        myclient.complete = false;

        int ngame = myclient.S2C_GAME_LIST_obj.ngame;
        // Debug.Log($"ngame: {ngame}");
        for (int i=0;i<ngame;i++){
            GameObject gameinfo = GameInfos.GetChild(i).gameObject;
            long sn = myclient.S2C_GAME_LIST_obj.gamedata_list[i].sn;
            byte kind = myclient.S2C_GAME_LIST_obj.gamedata_list[i].kind;
            short idxdata = myclient.S2C_GAME_LIST_obj.gamedata_list[i].idxdata;
            short id = myclient.S2C_GAME_LIST_obj.gamedata_list[i].id;
            byte type = myclient.S2C_GAME_LIST_obj.gamedata_list[i].type;
            short npigeon = myclient.S2C_GAME_LIST_obj.gamedata_list[i].npigeon;
            byte status = myclient.S2C_GAME_LIST_obj.gamedata_list[i].status;
            int time = myclient.S2C_GAME_LIST_obj.gamedata_list[i].time;

            sn2idx.Add(sn, i);
            sn2status.Add(sn, status);
            sn2require.Add(sn, openGameDatas[idxdata].requirePigeonNumber);

            gameinfo.SetActive(true);
            if (status == 0x03){
                gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GameOverSp;
                gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽結束";
                gameinfo.transform.Find("SignUpBtn").GetComponent<Button>().interactable = false;
                gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                // Debug.Log("比賽結束");
            }else if (status == 0x02){
                gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GamingSp;
                gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽中";
                gameinfo.transform.Find("SignUpBtn").GetComponent<Button>().interactable = false;
                gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                // Debug.Log("比賽中");
            }else if (status == 0x00){
                // gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GamingSp;
                // gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽中";
                gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                // Debug.Log("未達");
            }else if (status == 0x01){
                Debug.Log($"time: {time}");
                // Debug.Log("倒數");
            }
            



            gameinfo.transform.Find("GameIdx").GetComponent<Text>().text = sn.ToString();
            string gameName = openGameDatas[idxdata].name;
            gameinfo.transform.Find("GameName").GetComponent<Text>().text = gameName;
            int prize = (int)(openGameDatas[idxdata].requirePigeonNumber * openGameDatas[idxdata].fee * 0.95f);
            gameinfo.transform.Find("GamePrize").GetComponent<Text>().text = "獎金: " + prize.ToString();
            int pigeonRequiered = openGameDatas[idxdata].requirePigeonNumber;
            gameinfo.transform.Find("PigeonRequire").GetComponent<Text>().text = npigeon.ToString() + " / " + pigeonRequiered.ToString();
            string startPoint = openGameDatas[idxdata].startPoint;
            gameinfo.transform.Find("StartPoint").GetComponent<Text>().text = startPoint;
            string startCoef = "東經";
            startCoef += Mathf.Floor(openGameDatas[idxdata].startLng).ToString();
            startCoef += "度";
            startCoef += ((openGameDatas[idxdata].startLng * 100) % 100).ToString();
            startCoef += "分 北緯";
            startCoef += Mathf.Floor(openGameDatas[idxdata].startLat).ToString();
            startCoef += "度";
            startCoef += ((openGameDatas[idxdata].startLat * 100) % 100).ToString();
            startCoef += "分";
            int distance = getDistance(openGameDatas[idxdata].startLng, openGameDatas[idxdata].startLat, openGameDatas[idxdata].endLng, openGameDatas[idxdata].endLat);
            gameinfo.transform.Find("Distance").GetComponent<Text>().text = "距離: " + distance.ToString() + "km";
            gameinfo.transform.Find("Days").GetComponent<Text>().text = "天數: " + openGameDatas[idxdata].days + "天";
        }

        byte[] buffer = myclient.S2C_GAME_LIST_obj.raw;
        // Debug.Log("recvData: ");
        // myclient.printByte(buffer);
        myclient.S2C_GAME_UPDATE();
        StartCoroutine(waitForUpdate());
    }
    IEnumerator waitForUpdate()
    {
        while (true){
            if (myclient.gameUpdate){
                long sn = myclient.S2C_GAME_UPDATE_obj.sn;
                short npigeon = myclient.S2C_GAME_UPDATE_obj.npigeon;
                byte status = myclient.S2C_GAME_UPDATE_obj.status;
                int time = myclient.S2C_GAME_UPDATE_obj.time;

                Debug.Log("=== Game Update ===");
                Debug.Log($"sn: {sn}");
                Debug.Log($"npigeon: {npigeon}");
                Debug.Log($"status: {status}");
                Debug.Log($"time: {time}");
                // Debug.Log("raw: ");
                // myclient.printByte(myclient.S2C_GAME_UPDATE_obj.raw);

                GameObject gameinfo = GameInfos.GetChild(sn2idx[sn]).gameObject;
                gameinfo.transform.Find("PigeonRequire").GetComponent<Text>().text = npigeon.ToString() + " / " + sn2require[sn].ToString();


                if (sn2status[sn] != status){
                    
                    if (status == 0x03){
                        gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GameOverSp;
                        gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽結束";
                        gameinfo.transform.Find("SignUpBtn").GetComponent<Button>().interactable = false;
                        gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                        gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                    }else if (status == 0x02){
                        gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GamingSp;
                        gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽中";
                        gameinfo.transform.Find("SignUpBtn").GetComponent<Button>().interactable = false;
                        gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                        gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                    }else if (status == 0x00){
                        // gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = RegisterSp;
                        // gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "我要報名";
                        gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(false);
                        gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(false);
                    }else if (status == 0x01){
                        // gameinfo.transform.Find("SignUpBtn").GetComponent<Image>().sprite = GamingSp;
                        // gameinfo.transform.Find("SignUpBtn").Find("SignUpText").GetComponent<Text>().text = "比賽中";
                        gameinfo.transform.Find("SignUpBtn").Find("Mask").gameObject.SetActive(true);
                        gameinfo.transform.Find("SignUpBtn").Find("Time").gameObject.SetActive(true);
                        Debug.Log($"time: {time}");
                    }
                }

                myclient.gameUpdate = false;
            }else{
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnApplicationQuit()
    {
        myclient.serverSocket.Close();
        Debug.Log("quit");
    }
    int getDistance(float lng1, float lat1, float lng2, float lat2)
    {
        double EARTH_RADIUS = 6.371229*1e6;
        double radlng1 = Rad(lng1);
        double radlat1 = Rad(lat1);
        double radlng2 = Rad(lng2);
        double radlat2 = Rad(lat2);
        double a = radlat1 - radlat2;
        double b = radlng1 - radlng2;
        double result = 2 * Mathf.Asin(Mathf.Sqrt(Mathf.Pow(Mathf.Sin((float)(a / 2)), 2) + Mathf.Cos((float)radlat1) * Mathf.Cos((float)radlat2) * Mathf.Pow(Mathf.Sin((float)(b / 2)), 2))) * EARTH_RADIUS;
        return (int)(Mathf.Floor((float)(result / 1000)));
    }
    double Rad(double d)
    {
        return (double)d * Mathf.PI / 180f;
    }
}
