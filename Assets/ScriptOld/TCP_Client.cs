using UnityEngine;
using System.Collections;
// import
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;


public class TCP_Client: MonoBehaviour
{
    // string editString="hello wolrd"; //編輯框文字
    // string recvStr; //接收的字串
    // string sendStr; //傳送的字串
    // byte[] bbb;

    Socket serverSocket;
    IPAddress ip;
    IPEndPoint ipEnd; 
    byte[] recvData=new byte[1024];
    byte[] sendData=new byte[1024];
    int recvLen;
    Thread connectThread;

    // pigeon
    
    public pigeon[] pigeons;
    byte mark;
    byte version;
    byte command;
    ushort dataSize;
    int pigeonId;
    short nPath;

    Action[] path;
    int idx;

    byte action;
    int second;
    float y;
    float x;

    // data
    byte[] recvTotal = new byte[232768];
    int mergeIdx;

    int number;
    
    
    void Start()
    {
        // ip + port setting
        ip=IPAddress.Parse("59.124.3.23");
        ipEnd=new IPEndPoint(ip,22919);
        
        // StartCoroutine(foo());
        // create thread
        connectThread=new Thread(new ThreadStart(foo));
        connectThread.Start();

        StartCoroutine(ExampleCoroutine());
        
    }
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(3);
        parseData();
    }
    void con()
    {
        if(serverSocket!=null)
            serverSocket.Close();
    
        serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        print("ready to connect");
        serverSocket.Connect(ipEnd);
    }
    void foo()
    {
        con();


        // ---- test data ---- //
        sendData=new byte[3 + 2 + 17 * 4];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x30;


        ushort[] s = new ushort[1];
        s[0] = 17*4;
        Buffer.BlockCopy(s, 0, sendData, 3, 2);

        float[] f = new float[16];
        for (int i=0;i<16;i++){
            f[i] = 50.0f;
        }
        f[0] = 26.43f;
        f[1] = 122.27f;
        f[2] = 24.16f;
        f[3] = 120.65f;

        f[4] = -0.785f;
        f[7] = 24.5f;
        Buffer.BlockCopy(f, 0, sendData, 3 + 1*2, 16 * 4);

        int[] ii = new int[1];
        number = 50;
        ii[0] = number;
        Buffer.BlockCopy(ii, 0, sendData, 3 + 1*2 + 16*4, 1 * 4);
        // Debug.Log("sendData: ");
        // printByte(sendData);
        // ---- test data end --- //


        // send
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        // Debug.Log("send complete");
        mergeIdx = 0;
        pigeons = new pigeon[ii[0]];

        while (true)
        {
            recvData=new byte[2048];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                con();
                // Debug.Log("recv 0 data");
                // continue;
                
            } else {
                // printByte(recvData);
                // Debug.Log("recvLen: " + recvLen);
                merge();
                // parseData();
            }
        }
        
        
    }
    void merge()
    {
        Buffer.BlockCopy(recvData, 0, recvTotal, mergeIdx, recvLen);
        mergeIdx += recvLen;
    }
    void parseData()
    {
        
        command = 0x40;
        idx = 0;
        while (command != 0x41)
        {
            mark = recvTotal[idx];  idx += 1;
            // Debug.Log("mark: " + mark);
            version = recvTotal[idx];   idx += 1;
            // Debug.Log("version: " + version);
            command = recvTotal[idx];   idx += 1;
            // Debug.Log("command: " + command);
            dataSize = BitConverter.ToUInt16(recvTotal, idx);   idx += 2;
            // Debug.Log("dataSize: " + dataSize);
            // for (int j=0;j<number;j++)
            // {
            pigeonId = BitConverter.ToInt32(recvTotal, idx);    idx += 4;
            Debug.Log("pigeonId: " + pigeonId);
            nPath = BitConverter.ToInt16(recvTotal, idx);       idx += 2;
            Debug.Log("nPath: " + nPath);

            pigeons[pigeonId].path = new Action[nPath];
            for (int i=0;i<nPath;i++)
            {
                string action_dbg = pigeons[pigeonId].path[i].action = actionMap(recvTotal[idx]); idx += 1;
                // Debug.Log("action: " +  pigeons[pigeonId].path[i].action);
                int second_dbg = pigeons[pigeonId].path[i].second = BitConverter.ToInt32(recvTotal, idx); idx += 4;
                byte[] bb = new byte[4];
                Buffer.BlockCopy(recvTotal, idx - 4, bb, 0, 4);
                // printByte(bb);

                // Debug.Log("second: " + pigeons[pigeonId].path[i].second);
                float y_dbg = pigeons[pigeonId].path[i].y = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                // Debug.Log("y: " + pigeons[pigeonId].path[i].y);
                float x_dbg = pigeons[pigeonId].path[i].x = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                // Debug.Log("x: " + pigeons[pigeonId].path[i].x);
                if (action_dbg != "flyto" && action_dbg != "circle")
                {
                    Debug.Log("action: " +  action_dbg
                        + "  second: " + second_dbg
                        + "  y: " + y_dbg
                        + "  x: " + x_dbg);
                }
                
            }
        }
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
    void SocketQuit()
    {
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if(serverSocket!=null)
            serverSocket.Close();
        print("diconnect");
    }
    void OnApplicationQuit()
    {
        // printByte(recvTotal);
        Debug.Log("Total data: " + mergeIdx);
        // parseData();
        SocketQuit();
    }
}