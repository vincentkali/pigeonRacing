using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public struct pigeon
{
    public int pigeonId;
    public short nPath;
    public Action[] path; 
}
[Serializable]
public struct Action
{
    public string action;
    public int second;
    public float y;
    public float x;

}
[Serializable]
public struct PigeonAttributes
{
    public byte mark;
    public byte version;
    public byte command;
    public ushort dataSize;

    public float starting_point_lat;
    public float starting_point_lng;
    public float ending_point_lat;
    public float ending_point_lng;
    public float wind_dir;
    public float wind_speed;
    public float rain;
    public float air_temperature;
    public float speed;
    public float direction;
    public float stamina;
    public float hot_adaptability;
    public float cold_adaptability;
    public float rain_adaptability;
    public float wind_adaptability;
    public float intelligence;
    public int number;
}

[Serializable]
public struct S2C_LOGIN {
        public byte[] raw;
        public int pid;
        public int rkey;
        public string nickname;
    }

public struct S2C_PLAYER_PIGEON{
    public byte[] raw;
    public int npigeon;
    public pigeonAtt[] pigeonAtt_list;
    public string[] pigeonName_list;
}
public struct pigeonAtt{
    public long id;
    public float intelligence;
    public float vision;
    public float body;
    public float feather;
    public float endurance;
    public float muscle;
    public float stomach;
    public float waterproof;
    public float fatigue;
    public long sngame;
    public short age;
}
public struct S2C_PLAYER_DATA{
    public byte[] raw;
    public int coin;
}
public struct S2C_GAME_LIST{
    public byte[] raw;
    public int ngame; // self add
    public gameData[] gamedata_list;
}
public struct gameData{
    public long sn;
    public byte kind;
    public short idxdata;
    public short id;
    public byte type;
    public short npigeon;
    public byte status;
    public int time; 
}
public struct S2C_GAME_UPDATE{
    public byte[] raw;
    public long sn;
    public short npigeon;
    public byte status;
    public int time;
}

public struct S2C_PIGEON_ACTION{
    public byte[] raw;
    public long action;
    public long[] idpigeon;
    public int npigeon; // self add
}
public struct OpenGameData{
    public int idxdata;
    public string name;
    public string startPoint; // 起點
    public string endPoint; // 終點
    public float startLng; // 起點經度
    public float startLat; // 起點緯度
    public float endLng; // 終點經度
    public float endLat; // 終點緯度
    public int days; // 天數
    public int condition; // 條件
    public int fee; // 費用
    public int requirePigeonNumber; // 開賽鴿數
}













public class StructureDefine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
