using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    MyClient myclient;
    int pid;
    int rkey;
    bool next = false;
    public Text status;

    void Start()
    {
        myclient = StaticObject.myclient;
        string token = tokenGenerator();
        string name = token.Substring(0, 3);
        myclient.C2S_LOGIN_PLAY(token, name);
        status.text = "C2S_LOGIN_PLAY";

        StartCoroutine(waitForComplete1());
    }
    public void nextStepBtnClick()
    {
        next = true;
    }
    IEnumerator waitForComplete1()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;

        PlayerPrefs.SetInt("pid", myclient.S2C_LOGIN_obj.pid);
        PlayerPrefs.SetInt("rkey", myclient.S2C_LOGIN_obj.rkey);
        pid = PlayerPrefs.GetInt("pid");
        rkey = PlayerPrefs.GetInt("rkey");

        yield return new WaitForSeconds(1);
        while(next == false){
            yield return new WaitForSeconds(0.5f);
        }
        next = false;
        myclient.C2S_PLAYER_DATA(pid, rkey);
        status.text = "C2S_PLAYER_DATA";
        StartCoroutine(waitForComplete2());
    }
    IEnumerator waitForComplete2()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;

        // int coin = myclient.S2C_PLAYER_DATA_obj.coin;
        // Debug.Log("coin: " + coin.ToString());
        // byte[] buffer = myclient.S2C_PLAYER_DATA_obj.raw;

        yield return new WaitForSeconds(1);
        while(next == false){
            yield return new WaitForSeconds(0.5f);
        }
        next = false;
        myclient.C2S_PLAYER_PIGEON(pid, rkey);
        status.text = "C2S_PLAYER_PIGEON";
        StartCoroutine(waitForComplete3());
    }
    IEnumerator waitForComplete3()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;

        // int npigeon = myclient.S2C_PLAYER_PIGEON_obj.npigeon;
        // for (int i=0;i<npigeon;i++){
        //     Debug.Log($"id [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].id}");
        //     Debug.Log($"intelligence [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].intelligence}");
        //     Debug.Log($"vision [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].vision}");
        //     Debug.Log($"body [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].body}");
        //     Debug.Log($"feather [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].feather}");
        //     Debug.Log($"endurance [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].endurance}");
        //     Debug.Log($"muscle [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].muscle}");
        //     Debug.Log($"stomach [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].stomach}");
        //     Debug.Log($"waterproof [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].waterproof}");
        //     Debug.Log($"fatigue [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].fatigue}");
        //     Debug.Log($"sngame [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].sngame}");
        //     Debug.Log($"age [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].age}");
        //     Debug.Log($"name [{i}]: {myclient.S2C_PLAYER_PIGEON_obj.pigeonName_list[i]}");
        // }

        // byte[] buffer = myclient.S2C_PLAYER_PIGEON_obj.raw;
        // Debug.Log("recvData: ");
        // myclient.printByte(buffer);

        yield return new WaitForSeconds(1);
        while(next == false){
            yield return new WaitForSeconds(0.5f);
        }
        next = false;
        myclient.C2S_GAME_LIST(pid, rkey);
        status.text = "C2S_GAME_LIST";
        StartCoroutine(waitForComplete4());
    }
    IEnumerator waitForComplete4()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;

        // int N = myclient.S2C_GAME_LIST_obj.ngame;
        // for (int i=0;i<N;i++){
        //     Debug.Log($"sn [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].sn}");
        //     Debug.Log($"kind [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].kind}");
        //     Debug.Log($"idxdata [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].idxdata}");
        //     Debug.Log($"id [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].id}");
        //     Debug.Log($"type [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].type}");
        //     Debug.Log($"npigeon [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].npigeon}");
        //     Debug.Log($"status [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].status}");
        //     Debug.Log($"time [{i}]: {myclient.S2C_GAME_LIST_obj.gamedata_list[i].time}");
        // }
        long action = myclient.S2C_GAME_LIST_obj.gamedata_list[0].sn;
        long[] idpigeon = new long[3];
        for(int i=0; i<3;i++){
            idpigeon[i] = myclient.S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].id;
        }
        int npigeon = 3;

        myclient.C2S_PIGEON_ACTION(pid, rkey, action, idpigeon, npigeon);
        // while(next == false){
        //     yield return new WaitForSeconds(0.5f);
        // }
        // next = false;
        status.text = "C2S_PIGEON_ACTION";
        // yield return new WaitForSeconds(1);
        StartCoroutine(waitForComplete5());
    }
    IEnumerator waitForComplete5()
    {
        while (myclient.complete == false){
            yield return new WaitForSeconds(0.5f);
        }
        myclient.complete = false;

        // int npigeon = 3;
        // Debug.Log($"action : {myclient.S2C_PIGEON_ACTION_obj.action}");
        // for (int i=0; i<npigeon; i++){ 
        //     Debug.Log($"idpigeon [{i}]: {myclient.S2C_PIGEON_ACTION_obj.idpigeon[i]}");
        // }

        myclient.S2C_GAME_UPDATE();
        while(next == false){
            yield return new WaitForSeconds(0.5f);
        }
        next = false;
        status.text = "S2C_GAME_UPDATE";
        yield return new WaitForSeconds(1);
        StartCoroutine(waitForComplete6());
    }

    IEnumerator waitForComplete6()
    {
        // while (myclient.complete == false){
        //     yield return new WaitForSeconds(0.5f);
        // }
        // myclient.complete = false;

        while(true){
            if (myclient.gameUpdate){
                Debug.Log($"sn : {myclient.S2C_GAME_UPDATE_obj.sn}");
                Debug.Log($"npigeon : {myclient.S2C_GAME_UPDATE_obj.npigeon}");
                Debug.Log($"status : {myclient.S2C_GAME_UPDATE_obj.status}");
                Debug.Log($"time : {myclient.S2C_GAME_UPDATE_obj.time}");

                myclient.gameUpdate = false;
            }else{
                yield return new WaitForSeconds(0.5f);
            }
        }

        // myclient.C2S_GAME_LIST(pid, rkey);
        // yield return new WaitForSeconds(1);
        // StartCoroutine(waitForComplete5());
    }

    void OnApplicationQuit()
    {
        StopAllCoroutines();
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
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
