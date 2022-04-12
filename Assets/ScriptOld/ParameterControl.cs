using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ParameterControl : MonoBehaviour
{
    # region Data: UI Parameter
    [Header("---- first block ----")]
    public Text starting_point_1_text;  public Text starting_point_1_placeholder;  private string starting_point_1;
    public Text starting_point_2_text;  public Text starting_point_2_placeholder;  private string starting_point_2;
    public Text ending_point_1_text;    public Text ending_point_1_placeholder;    private string ending_point_1;
    public Text ending_point_2_text;    public Text ending_point_2_placeholder;    private string ending_point_2;
    public Text wind_1_text;            public Text wind_1_placeholder;            private float wind_1;
    public Text wind_2_text;            public Text wind_2_placeholder;            private int wind_2;
    public Text rain_text;              public Text rain_placeholder;              private string rain;
    public Text air_temperature_text;   public Text air_temperature_placeholder;   private float air_temperature;
    [Header("---- second block ----")]
    public Text speed_text;             public Text speed_placeholder;             private float speed;
    public Text direction_text;         public Text direction_placeholder;         private float direction;
    public Text stamina_text;           public Text stamina_placeholder;           private float stamina;
    public Text hot_adaptability_text;  public Text hot_adaptability_placeholder;  private float hot_adaptability;
    public Text cold_adaptability_text; public Text cold_adaptability_placeholder; private float cold_adaptability;
    public Text rain_adaptability_text; public Text rain_adaptability_placeholder; private float rain_adaptability;
    public Text wind_adaptability_text; public Text wind_adaptability_placeholder; private float wind_adaptability;
    public Text intelligence_text;      public Text intelligence_placeholder;      private float intelligence;
    [Header("---- third block ----")]
    public Text number_text;            public Text number_placeholder;            public int number;
    public Button startButton;
    public Text errorMsg;

    // Data Type Convert
    public float starting_1;
    public float starting_2;
    public float ending_1;
    public float ending_2;
    public float rain_float;

    // Parameter Correctness Checking
    private bool canStart;
    # endregion
    
    # region Data: Cross Scene Data
    public Action[] path;
    public Action[][] pigeonsPath; // [pigeonId][pathId]
    public PigeonAttributes pigeonAttributes;
    # endregion
    
    # region Data: Socket
    Socket serverSocket; //伺服器端socket
    IPAddress ip; //主機ip
    IPEndPoint ipEnd; 
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData=new byte[10000]; //接收的資料，必須為位元組
    byte[] sendData=new byte[1024]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread; //連線執行緒
    # endregion

    # region Data: Parse Path Data
    // Basic Parameter
    byte mark;
    byte version;
    byte command;
    ushort dataSize;
    int pigeonId;
    short nPath;
    int idx;

    // Data Structure
    public pigeon[] pigeons;

    // Merging Used
    int mergeIdx;
    byte[] recvTotal = new byte[10000000];

    // Status Control
    bool canParse;
    bool ParsePathDone;
    # endregion
    
    # region Data: Parse Wind data
    public float[][] wind_speed = new float[23][]; // 23 x 40
    public float[][] wind_direction = new float[23][]; // 23 x 40
    int totalWindRecvData;
    bool getWindDataDone;
    byte kind;
    short dataLen;
    bool ParseWindDataDone;
    # endregion
    # region Data: Test
    // Test Used
    public Boolean printSendData;
    public Boolean printRecvData;
    public Boolean useTestData;
    public int testDataNumber;
    # endregion
    bool getWindSpeedDataDone;
    bool canLoadScene;
    bool ParseWindSpeedDataDone;
    bool getWindDriectionDataDone;
    bool parseWindDriectionDataDone;

    # region Function: Starting
    void Awake() 
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    void Start()
    {
        startButton.onClick.AddListener(startButtonOnClick);
        canStart = true;
        canParse = false;
        ParsePathDone = false;
        getWindDataDone = false;
        ParseWindDataDone = false;
        canLoadScene = false;
        getWindSpeedDataDone = false;
        ParseWindSpeedDataDone = false;
        getWindDriectionDataDone = false;
        parseWindDriectionDataDone = false;

        ip=IPAddress.Parse("59.124.3.23");
        ipEnd=new IPEndPoint(ip,22919);
        
        for (int i=0;i<23;i++){
            wind_speed[i] = new float[40];
            wind_direction[i] = new float[40];
        }
    }
    void startButtonOnClick(){
        // get user input, and check corret or not. 
        // if not correct, output the error info, which has format, range, example
        getInput();
        checkInput();
        if (canStart)
        {
            writePacket(); // write C2S packet

            // SocketConnet();
            if (useTestData){
                loadTestPathData();
                loadSimulationScene();
            }
            else {
                getPath();
                StartCoroutine(StartPathDataPipeline());
            }
            
        }
    }
    IEnumerator StartPathDataPipeline()
    {
        // Wait for Data Correctness Checking
        float start = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (canParse) break;
            if (Time.time - start > 3) break;
        }
        yield return new WaitForSeconds(1);

        // Start Parsing
        SocketQuit();
        parsePathData();

        // Wait for Parsing End
        start = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (ParsePathDone) break;
            if (Time.time - start > 3) break;
        }
        yield return new WaitForSeconds(1);

        getWindSpeedData();

        // Wait for Wind data Parsing
        start = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (getWindSpeedDataDone) break;
            if (Time.time - start > 20) break;
        }
        yield return new WaitForSeconds(1);

        SocketQuit();
        ParseWindData("speed");

        start = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (ParseWindSpeedDataDone) break;
            if (Time.time - start > 20) break;
        }
        yield return new WaitForSeconds(1);

        getWindDirectionData();

        start = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (getWindDriectionDataDone) break;
            if (Time.time - start > 20) break;
        }
        yield return new WaitForSeconds(1);

        SocketQuit();
        ParseWindData("direction");

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (parseWindDriectionDataDone) break;
            if (Time.time - start > 20) break;
        }
        yield return new WaitForSeconds(1);

        // Load Scene
        loadSimulationScene();
    }
    # endregion

    # region Function: Send/Recv Data Handling
    void getWindDirectionData()
    {
        connectThread=new Thread(new ThreadStart(PrepareClientWindDirectionData));
        connectThread.Start();
    }
    void PrepareClientWindDirectionData()
    {
        connection();

        sendData=new byte[3 + 2 + 1];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x50;


        ushort[] s = new ushort[1];
        s[0] = 1;
        Buffer.BlockCopy(s, 0, sendData, 3, 2);

        byte[] b = new byte[1];
        b[0] = 0x01; // 0: wind_speed, 1: wind_direction, 2: temperature, 3: rain, 4: visibility
        Buffer.BlockCopy(b, 0, sendData, 3 + 1*2, 1);

        // Debug.Log("sendData: ");
        printByte(sendData);
        // ---- test data end --- //

        mergeIdx = 0;
        // send
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        // Debug.Log("send complete");

        while (true)
        {
            recvData=new byte[2048];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                connection();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                // printByte(recvData);
                // Debug.Log("recvLen: " + recvLen);
                merge();
                totalWindRecvData += recvLen;
                if (totalWindRecvData >= 920*4 + 6)
                {
                    Debug.Log("Wind Driection Data Recv End");
                    break;
                }
                // parseData();
            }
        }
        getWindDriectionDataDone = true;
    }
    void ParseWindData(string kindness)
    {
        Debug.Log("Parse Start");
        printByte(recvTotal);
        command = 0x50;
        idx = 0;
        
        mark = recvTotal[idx];  idx += 1;
        // Debug.Log("mark: " + mark);
        version = recvTotal[idx];   idx += 1;
        // Debug.Log("version: " + version);
        command = recvTotal[idx];   idx += 1;
        // Debug.Log("command: " + command);
        kind = recvTotal[idx]; idx += 1;
        dataLen = BitConverter.ToInt16(recvTotal, idx);       idx += 2;

        if (kindness == "speed")
        {
            for (int i=0;i<23;i++){
                wind_speed[i] = new float[40];
                for (int j=0;j<40;j++){
                    wind_speed[i][j] = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                    //Debug.Log("wind_speed (" + i + " , " + j + " ): " + wind_speed[i][j]);
                }
            }
            Debug.Log("Parse Wind Speed End");
            ParseWindSpeedDataDone = true;
        }
        else if (kindness == "direction")
        {
            for (int i=0;i<23;i++){
                wind_direction[i] = new float[40];
                for (int j=0;j<40;j++){
                    wind_direction[i][j] = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                    //Debug.Log("wind_direction (" + i + " , " + j + " ): " + wind_direction[i][j]);
                }
            }
            Debug.Log("Parse Wind Direction end");
            parseWindDriectionDataDone = true;
        }
        else{
            Debug.Log("kindness error");
        }
        
        
        
    }
    void getWindSpeedData()
    {
        connectThread=new Thread(new ThreadStart(PrepareClientWindSpeedData));
        connectThread.Start();
    }
    void PrepareClientWindSpeedData()
    {
        connection();

        sendData=new byte[3 + 2 + 1];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x50;


        ushort[] s = new ushort[1];
        s[0] = 1;
        Buffer.BlockCopy(s, 0, sendData, 3, 2);

        byte[] b = new byte[1];
        b[0] = 0x00; // 0: wind_speed, 1: wind_direction, 2: temperature, 3: rain, 4: visibility
        Buffer.BlockCopy(b, 0, sendData, 3 + 1*2, 1);

        Debug.Log("sendData: ");
        printByte(sendData);
        // ---- test data end --- //

        mergeIdx = 0;
        // send
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        Debug.Log("send complete");

        while (true)
        {
            recvData=new byte[2048];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                connection();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                // printByte(recvData);
                // Debug.Log("recvLen: " + recvLen);
                merge();
                totalWindRecvData += recvLen;
                if (totalWindRecvData >= 920*4 + 6)
                {
                    Debug.Log("Wind Speed Data Recv End");
                    break;
                }
                // parseData();
            }
        }
        getWindSpeedDataDone = true;
    }
    
    void writePacket()
    {

        pigeonAttributes.mark = 0xc8;
        pigeonAttributes.version = 0x01;
        pigeonAttributes.command = 0x30;

        pigeonAttributes.dataSize = 17 * 4;

        pigeonAttributes.starting_point_lat = starting_1;
        pigeonAttributes.starting_point_lng = starting_2;
        pigeonAttributes.ending_point_lat = ending_1;
        pigeonAttributes.ending_point_lng = ending_2;

        pigeonAttributes.wind_dir = wind_1;
        pigeonAttributes.wind_speed = wind_2;
        pigeonAttributes.rain = rain_float;
        pigeonAttributes.air_temperature = air_temperature;
        
        pigeonAttributes.speed = speed;
        pigeonAttributes.direction = direction;
        pigeonAttributes.stamina = stamina;
        pigeonAttributes.hot_adaptability = hot_adaptability;

        pigeonAttributes.cold_adaptability = cold_adaptability;
        pigeonAttributes.rain_adaptability = rain_adaptability;
        pigeonAttributes.wind_adaptability = wind_adaptability;
        pigeonAttributes.intelligence = intelligence;

        pigeonAttributes.number = number;
    }
    void PrepareClientPathData()
    {
        // Prepare C2S Data
        connection();

        // ---- test data ---- //
        sendData=new byte[3 + 2 + 17 * 4];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x30;


        ushort[] s = new ushort[1];
        s[0] = 17*4;
        Buffer.BlockCopy(s, 0, sendData, 3, 2);

        float[] f = new float[16];
        f[0] = pigeonAttributes.starting_point_lat;
        f[1] = pigeonAttributes.starting_point_lng;
        f[2] = pigeonAttributes.ending_point_lat;
        f[3] = pigeonAttributes.ending_point_lng;

        f[4] = pigeonAttributes.wind_dir;
        f[5] = pigeonAttributes.wind_speed;
        f[6] = pigeonAttributes.rain;
        f[7] = pigeonAttributes.air_temperature;
        
        f[8] = pigeonAttributes.speed;
        f[9] = pigeonAttributes.direction;
        f[10] = pigeonAttributes.stamina;
        f[11] = pigeonAttributes.hot_adaptability;
        
        f[12] = pigeonAttributes.cold_adaptability;
        f[13] = pigeonAttributes.rain;
        f[14] = pigeonAttributes.wind_adaptability;
        f[15] = pigeonAttributes.intelligence;

        Buffer.BlockCopy(f, 0, sendData, 3 + 1*2, 16 * 4);

        int[] ii = new int[1];
        ii[0] = pigeonAttributes.number;
        Buffer.BlockCopy(ii, 0, sendData, 3 + 1*2 + 16*4, 1 * 4);
        if (printSendData) Debug.Log("sendData: ");
        // printByte(sendData);
        // ---- test data end --- //


        // send
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        if (printSendData) Debug.Log("send complete");
        mergeIdx = 0;
        pigeons = new pigeon[ii[0]];

        while (true)
        {
            recvData=new byte[10000];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                connection();
                if (printRecvData) Debug.Log("recv 0 data");
                // continue;
                
            } else {
                if (printRecvData) Debug.Log("recvLen: " + recvLen);
                merge();
                // printByte(recvData);
                if (recvData[0] == 0x8c && recvData[1] == 0x01 && recvData[2] == 0x41)
                {
                    Debug.Log("Data Pipeline Status: Socket Recv Done (get end signal)");
                    canParse = true;
                }
            }
        }
    }
    void parsePathData()
    {
        // Parse Data: print / store to Data Structure pigeons
        command = 0x40;
        idx = 0;
        while (true)
        {
            mark = recvTotal[idx];  idx += 1;
            if (printRecvData) Debug.Log("mark: " + mark);
            version = recvTotal[idx];   idx += 1;
            if (printRecvData) Debug.Log("version: " + version);
            command = recvTotal[idx];   idx += 1;
            
            if (command == 0x41) break;

            if (printRecvData) Debug.Log("command: " + command);
            dataSize = BitConverter.ToUInt16(recvTotal, idx);   idx += 2;
            if (printRecvData) Debug.Log("dataSize: " + dataSize);
            pigeonId = BitConverter.ToInt32(recvTotal, idx);    idx += 4;
            pigeons[pigeonId].pigeonId = pigeonId;
            if (printRecvData) Debug.Log("pigeonId: " + pigeonId);
            nPath = BitConverter.ToInt16(recvTotal, idx);       idx += 2;
            pigeons[pigeonId].nPath = nPath;
            if (printRecvData) Debug.Log("nPath: " + nPath);

            pigeons[pigeonId].path = new Action[nPath];
            for (int i=0;i<nPath;i++)
            {

                string action_dbg = pigeons[pigeonId].path[i].action = actionMap(recvTotal[idx]); idx += 1;
                if (printRecvData) Debug.Log("action: " +  pigeons[pigeonId].path[i].action);
                int secnod_dbg = pigeons[pigeonId].path[i].second = BitConverter.ToInt32(recvTotal, idx); idx += 4;
                byte[] bb = new byte[4];
                Buffer.BlockCopy(recvTotal, idx - 4, bb, 0, 4);
                if (printRecvData) printByte(bb);

                if (printRecvData) Debug.Log("second: " + pigeons[pigeonId].path[i].second );
                float y_dbg = pigeons[pigeonId].path[i].y = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                if (printRecvData) Debug.Log("y: " + pigeons[pigeonId].path[i].y);
                float x_dbg = pigeons[pigeonId].path[i].x = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                if (printRecvData) Debug.Log("x: " + pigeons[pigeonId].path[i].x);
                if (action_dbg  == "start")
                {
                    // Debug.Log("action: " +  action_dbg
                    //     + "  second: " + secnod_dbg
                    //     + "  y: " + y_dbg
                    //     + "  x: " + x_dbg);
                }
                
            }
        }
        Debug.Log("Data Pipeline Status: Parse Done");
        ParsePathDone = true;
    }
    # endregion
    
    # region Function: Socket
    void getPath()
    {
        // Get Path Data from Remote Server
        connectThread=new Thread(new ThreadStart(PrepareClientPathData));
        connectThread.Start();
    }
    void connection()
    {
        // Connect to Server
        if(serverSocket!=null)
            serverSocket.Close();
    
        serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        print("Socket Status: ready to connect");
        serverSocket.Connect(ipEnd);
    }
    void SocketQuit()
    {
        //關閉執行緒
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
        if(serverSocket!=null)
            serverSocket.Close();
        print("Socket Status: diconnect");
    }
    # endregion
    
    # region Function: Testing Used
    void loadTestPathData()
    {
        number = testDataNumber;
        pigeons = new pigeon[testDataNumber];

        int timeScale = 30;
        for (int i=0;i<testDataNumber;i++){
            pigeons[i].pigeonId = i;
            pigeons[i].nPath = Convert.ToInt16(5);
            pigeons[i].path = new Action[5];

            pigeons[i].path[0].action = "start";
            pigeons[i].path[0].second = 0;
            pigeons[i].path[0].x = MapToGameX(pigeonAttributes.starting_point_lng) + 10 * i;
            pigeons[i].path[0].y = MapToGameY(pigeonAttributes.starting_point_lat) + 10 * i;
            
            pigeons[i].path[1].action = "flyto";
            pigeons[i].path[1].second = 2 * timeScale;
            pigeons[i].path[1].x = MapToGameX(pigeonAttributes.ending_point_lng);
            pigeons[i].path[1].y = MapToGameY(pigeonAttributes.ending_point_lat);

            pigeons[i].path[2].action = "fail";
            pigeons[i].path[2].second = 2 * timeScale;

        }
    }
    # endregion

    # region Function: UI Input Handling
    void checkInput()
    {
        canStart = true;
        
        errorMsg.text = "";
        errorMsg.color = new Color(1,0,0,1);
        // ---- first block ----
        if (starting_point_1[0] != 'N' || !float.TryParse(starting_point_1.Split('N')[1] , out starting_1) ){
            errorMsg.text = "起點格式錯誤，範例:N26.43 , E122.27";
            canStart = false;
            return;
        }else {
            if (starting_1 < 20.5f || starting_1 > 27.5f){
                errorMsg.text = "起點超出範圍，需在 N20.5 ~ N27.5 之中";
                canStart = false;
                return;
            }
        }
        if (starting_point_2[0] != 'E' || !float.TryParse(starting_point_2.Split('E')[1] , out starting_2) ){
            errorMsg.text = "起點格式錯誤，範例:N26.43 , E122.27";
            canStart = false;
            return;
        }else {
            if (starting_2 < 118.5f || starting_2 > 123.0f){
                errorMsg.text = "起點超出範圍，需在 E118.5 ~ E123 之中";
                canStart = false;
                return;
            }
        }
        if (ending_point_1[0] != 'N' || !float.TryParse(ending_point_1.Split('N')[1] , out ending_1) ){
            errorMsg.text = "終點格式錯誤，範例:N24.16 , E120.65";
            canStart = false;
            return;
        }else {
            if (ending_1 < 20.5f || ending_1 > 27.5f){
                errorMsg.text = "終點超出範圍，需在 N20.5 ~ N27.5 之中";
                canStart = false;
                return;
            }
        }
        if (ending_point_2[0] != 'E' || !float.TryParse(ending_point_2.Split('E')[1] , out ending_2) ){
            errorMsg.text = "終點格式錯誤，範例:N24.16 , E120.65";
            canStart = false;
            return;
        }else {
            if (ending_2 < 118.5f || ending_2 > 123.0f){
                errorMsg.text = "終點超出範圍，需在 E118.5 ~ E123 之中";
                canStart = false;
                return;
            }
        }

        if (wind_1 < Mathf.PI * -1 || wind_1 > Mathf.PI){
            errorMsg.text = "風的第一欄超出範圍，需在 -3.141 ~ 3.141 之中";
            canStart = false;
            return;
        }
        if (wind_2 < 1.0f  || wind_2 > 99.99f){
            errorMsg.text = "風的第二欄超出範圍，需在 1 ~ 99 之中";
            canStart = false;
            return;
        }
        if (rain[rain.Length - 1] != '%' || !float.TryParse(rain.Split('%')[0], out rain_float)){
            errorMsg.text = "雨格式錯誤，範例: 50%";
            canStart = false;
            return;
        }else {
            if (rain_float < 1.0f || rain_float > 99.99f){
                errorMsg.text = "雨超出範圍，需在 1% ~ 99% 之中";
                canStart = false;
                return;
            }
        }

        
        
        if (air_temperature < 1.0f  || air_temperature > 99.99f){
            errorMsg.text = "氣溫超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }

        // ---- second block ----
        if (speed < 1.0f  || speed > 99.99f){
            errorMsg.text = "速度超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        if (direction < 1.0f  || direction > 99.99f){
            errorMsg.text = "方向超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        if (stamina < 1.0f  || stamina > 99.99f){
            errorMsg.text = "體力超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }

        if (hot_adaptability < 1.0f  || hot_adaptability > 99.99f){
            errorMsg.text = "熱適應超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        if (cold_adaptability < 1.0f  || cold_adaptability > 99.99f){
            errorMsg.text = "冷適應超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        if (rain_adaptability < 1.0f  || rain_adaptability > 99.99f){
            errorMsg.text = "雨適應超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        if (wind_adaptability < 1.0f  || wind_adaptability > 99.99f){
            errorMsg.text = "風適應超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }

        if (intelligence < 1.0f  || intelligence > 99.99f){
            errorMsg.text = "智力超出範圍，需在 1.0 ~ 99.99 之中";
            canStart = false;
            return;
        }
        // ---- third block ----
        if (number < 1.0f  || number > 99.99f){
            errorMsg.text = "隻數超出範圍，需在 1 ~ 99 之中";
            canStart = false;
            return;
        }
        
        if (canStart){
            Debug.Log("Data Pipeline Status: Input Checking Done");
        }
        else {
            Debug.Log("Data Pipeline Status: Input Checking Error");
        }
        errorMsg.text = "成功發送，等待資料...";
        errorMsg.color = new Color(0,1,0,1);
        
    } 
    void getInput()
    {
        starting_point_1 = starting_point_1_text.text.ToString().Length == 0 ? starting_point_1_placeholder.text.ToString() : starting_point_1_text.text.ToString();
        starting_point_2 = starting_point_2_text.text.ToString().Length == 0 ? starting_point_2_placeholder.text.ToString() : starting_point_2_text.text.ToString();
        ending_point_1 = ending_point_1_text.text.ToString().Length == 0 ? ending_point_1_placeholder.text.ToString() : ending_point_1_text.text.ToString();
        ending_point_2 = ending_point_2_text.text.ToString().Length == 0 ? ending_point_2_placeholder.text.ToString() : ending_point_2_text.text.ToString();

        wind_1 = wind_1_text.text.ToString().Length == 0 ? float.Parse(wind_1_placeholder.text.ToString())  : float.Parse(wind_1_text.text.ToString());
        wind_2 = wind_2_text.text.ToString().Length == 0 ? int.Parse(wind_2_placeholder.text.ToString())  : int.Parse(wind_2_text.text.ToString());
        rain = rain_text.text.ToString().Length == 0 ? rain_placeholder.text.ToString()  : rain_text.text.ToString();
        air_temperature = air_temperature_text.text.ToString().Length == 0 ? float.Parse(air_temperature_placeholder.text.ToString())  : float.Parse(air_temperature_text.text.ToString());

        speed = speed_text.text.ToString().Length == 0 ? float.Parse(speed_placeholder.text.ToString())  : float.Parse(speed_text.text.ToString());
        direction = direction_text.text.ToString().Length == 0 ? float.Parse(direction_placeholder.text.ToString())  : float.Parse(direction_text.text.ToString());
        stamina = stamina_text.text.ToString().Length == 0 ? float.Parse(stamina_placeholder.text.ToString())  : float.Parse(stamina_text.text.ToString());
        hot_adaptability = hot_adaptability_text.text.ToString().Length == 0 ? float.Parse(hot_adaptability_placeholder.text.ToString())  : float.Parse(hot_adaptability_text.text.ToString());

        cold_adaptability = cold_adaptability_text.text.ToString().Length == 0 ? float.Parse(cold_adaptability_placeholder.text.ToString())  : float.Parse(cold_adaptability_text.text.ToString());
        rain_adaptability = rain_adaptability_text.text.ToString().Length == 0 ? float.Parse(rain_adaptability_placeholder.text.ToString())  : float.Parse(rain_adaptability_text.text.ToString());
        wind_adaptability = wind_adaptability_text.text.ToString().Length == 0 ? float.Parse(wind_adaptability_placeholder.text.ToString())  : float.Parse(wind_adaptability_text.text.ToString());
        intelligence = intelligence_text.text.ToString().Length == 0 ? float.Parse(intelligence_placeholder.text.ToString())  : float.Parse(intelligence_text.text.ToString());

        number = number_text.text.ToString().Length == 0 ? int.Parse(number_placeholder.text.ToString())  : int.Parse(number_text.text.ToString());
    }
    # endregion

    # region Function: Other 
    float MapToGameX(float E)
    {
        // origin: 120.75 -> 0
        // size: 4.5 -> 450 
        return (E - 120.75f) * 200;
    }
    float MapToGameY(float N)
    {
        // origin: 24 -> 0
        // size: 7 -> 700
        return (N - 24.0f) * 200;
    }
    string actionMap(byte b)
    {
        if (b == 0x10){
            return "start";
        } else if (b == 0x20){
            return "circle";
        } else if (b == 0x30){
            return "flyto";
        } else if (b == 0x40){
            return "arrive";
        } else if (b == 0x50){
            return "fail";
        } else {
            return "error";
        }
    }
    void printByte(byte[] b)
    {
        Debug.Log(BitConverter.ToString(b));
    }
    void merge()
    {
        // Merge into recvTotal[]
        Buffer.BlockCopy(recvData, 0, recvTotal, mergeIdx, recvLen);
        mergeIdx += recvLen;
    }
    void loadSimulationScene()
    {
        GameObject.Find("Main Camera").gameObject.SetActive(false);
        GameObject.Find("Directional Light").gameObject.SetActive(false);
        GameObject.Find("Canvas").gameObject.SetActive(false);
        GameObject.Find("EventSystem").gameObject.SetActive(false);
        SceneManager.LoadScene("Simulation", LoadSceneMode.Additive);
    }
    # endregion

    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
