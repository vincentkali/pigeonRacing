using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticObject : MonoBehaviour
{
    public static MyClient myclient;
    public static OpenGameData[] openGameDatas;
    public static Dictionary<long, int> sn2idx = new Dictionary<long, int>();
    public static Dictionary<long, byte> sn2status = new Dictionary<long, byte>();
    public static Dictionary<long, int> sn2require = new Dictionary<long, int>();
    public static GameObject selectGameInfo;
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        myclient = new MyClient();
        Debug.Log("Create myclient");

        openGameDataInit();
    }
    static void openGameDataInit()
    {
        openGameDatas = new OpenGameData[10 + 1];
        openGameDatas[1].idxdata = 1;
        openGameDatas[1].name = "北海PK賽";
        openGameDatas[1].startPoint = "台北港";
        openGameDatas[1].endPoint = "台中"; // same
        openGameDatas[1].startLng = 121.37f;
        openGameDatas[1].startLat = 25.15f;
        openGameDatas[1].endLng = 120.64f; // same
        openGameDatas[1].endLat = 24.16f; // same
        openGameDatas[1].days = 10;
        openGameDatas[1].condition = 365; // same
        openGameDatas[1].fee = 1000; // same
        openGameDatas[1].requirePigeonNumber = 10;
        
        openGameDatas[2].idxdata = 2;
        openGameDatas[2].name = "情人PK賽";
        openGameDatas[2].startPoint = "基隆嶼";
        openGameDatas[2].endPoint = "台中"; // same
        openGameDatas[2].startLng = 121.78f;
        openGameDatas[2].startLat = 25.19f;
        openGameDatas[2].endLng = 120.64f; // same
        openGameDatas[2].endLat = 24.16f; // same
        openGameDatas[2].days = 12;
        openGameDatas[2].condition = 365; // same
        openGameDatas[2].fee = 1000; // same
        openGameDatas[2].requirePigeonNumber = 7;
        
        openGameDatas[3].idxdata = 3;
        openGameDatas[3].name = "清明PK賽";
        openGameDatas[3].startPoint = "基隆嶼";
        openGameDatas[3].endPoint = "台中"; // same
        openGameDatas[3].startLng = 121.78f;
        openGameDatas[3].startLat = 25.19f;
        openGameDatas[3].endLng = 120.64f; // same
        openGameDatas[3].endLat = 24.16f; // same
        openGameDatas[3].days = 12;
        openGameDatas[3].condition = 365; // same
        openGameDatas[3].fee = 1000; // same
        openGameDatas[3].requirePigeonNumber = 7;
        
        openGameDatas[4].idxdata = 4;
        openGameDatas[4].name = "初春PK賽";
        openGameDatas[4].startPoint = "漁人碼頭";
        openGameDatas[4].endPoint = "台中"; // same
        openGameDatas[4].startLng = 121.4f;
        openGameDatas[4].startLat = 25.18f;
        openGameDatas[4].endLng = 120.64f; // same
        openGameDatas[4].endLat = 24.16f; // same
        openGameDatas[4].days = 10;
        openGameDatas[4].condition = 365; // same
        openGameDatas[4].fee = 1000; // same
        openGameDatas[4].requirePigeonNumber = 7;
        
        openGameDatas[5].idxdata = 5;
        openGameDatas[5].name = "雨季競速賽";
        openGameDatas[5].startPoint = "漁人碼頭";
        openGameDatas[5].endPoint = "台中"; // same
        openGameDatas[5].startLng = 121.4f;
        openGameDatas[5].startLat = 25.18f;
        openGameDatas[5].endLng = 120.64f; // same
        openGameDatas[5].endLat = 24.16f; // same
        openGameDatas[5].days = 10;
        openGameDatas[5].condition = 365; // same
        openGameDatas[5].fee = 1000; // same
        openGameDatas[5].requirePigeonNumber = 10;
        
        openGameDatas[6].idxdata = 6;
        openGameDatas[6].name = "秋季北海PK賽";
        openGameDatas[6].startPoint = "彭佳嶼";
        openGameDatas[6].endPoint = "台中"; // same
        openGameDatas[6].startLng = 122.07f;
        openGameDatas[6].startLat = 25.63f;
        openGameDatas[6].endLng = 120.64f; // same
        openGameDatas[6].endLat = 24.16f; // same
        openGameDatas[6].days = 15;
        openGameDatas[6].condition = 365; // same
        openGameDatas[6].fee = 1000; // same
        openGameDatas[6].requirePigeonNumber = 10;
        
        openGameDatas[7].idxdata = 7;
        openGameDatas[7].name = "冬季北海PK賽";
        openGameDatas[7].startPoint = "彭佳嶼";
        openGameDatas[7].endPoint = "台中"; // same
        openGameDatas[7].startLng = 122.07f;
        openGameDatas[7].startLat = 25.63f;
        openGameDatas[7].endLng = 120.64f; // same
        openGameDatas[7].endLat = 24.16f; // same
        openGameDatas[7].days = 15;
        openGameDatas[7].condition = 365; // same
        openGameDatas[7].fee = 1000; // same
        openGameDatas[7].requirePigeonNumber = 10;
        
        openGameDatas[8].idxdata = 8;
        openGameDatas[8].name = "晴晴北海PK賽";
        openGameDatas[8].startPoint = "台北港";
        openGameDatas[8].endPoint = "台中"; // same
        openGameDatas[8].startLng = 121.37f;
        openGameDatas[8].startLat = 25.15f;
        openGameDatas[8].endLng = 120.64f; // same
        openGameDatas[8].endLat = 24.16f; // same
        openGameDatas[8].days = 10;
        openGameDatas[8].condition = 365; // same
        openGameDatas[8].fee = 1000; // same
        openGameDatas[8].requirePigeonNumber = 10;
        
        openGameDatas[9].idxdata = 9;
        openGameDatas[9].name = "春季北海PK賽";
        openGameDatas[9].startPoint = "漁人碼頭";
        openGameDatas[9].endPoint = "台中"; // same
        openGameDatas[9].startLng = 121.4f;
        openGameDatas[9].startLat = 25.18f;
        openGameDatas[9].endLng = 120.64f; // same
        openGameDatas[9].endLat = 24.16f; // same
        openGameDatas[9].days = 12;
        openGameDatas[9].condition = 365; // same
        openGameDatas[9].fee = 1000; // same
        openGameDatas[9].requirePigeonNumber = 10;
        
        openGameDatas[10].idxdata = 10;
        openGameDatas[10].name = "北海競翔賽";
        openGameDatas[10].startPoint = "漁人碼頭";
        openGameDatas[10].endPoint = "台中"; // same
        openGameDatas[10].startLng = 121.4f;
        openGameDatas[10].startLat = 25.18f;
        openGameDatas[10].endLng = 120.64f; // same
        openGameDatas[10].endLat = 24.16f; // same
        openGameDatas[10].days = 12;
        openGameDatas[10].condition = 365; // same
        openGameDatas[10].fee = 1000; // same
        openGameDatas[10].requirePigeonNumber = 10;
    }   
}