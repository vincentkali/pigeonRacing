using UnityEngine;
using System.Collections;
// import
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;


public class TCP_Client1: MonoBehaviour
{
    Socket serverSocket;
    IPAddress ip;
    IPEndPoint ipEnd; 
    byte[] recvData=new byte[1024];
    byte[] sendData=new byte[1024];
    int recvLen;
    Thread connectThread;

    // pigeon
    
    byte mark;
    byte version;
    byte command;
    int idx;
    // data
    byte[] recvTotal = new byte[232768];
    int mergeIdx;
    byte kind;
    float[][] wind_speed = new float[23][];
    short dataLen;
    int totalRecvLen;
    
    
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
        sendData=new byte[3 + 2 + 1];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x50;


        ushort[] s = new ushort[1];
        s[0] = 1;
        Buffer.BlockCopy(s, 0, sendData, 3, 2);

        byte[] b = new byte[1];
        b[0] = 0x00;
        Buffer.BlockCopy(b, 0, sendData, 3 + 1*2, 1);

        Debug.Log("sendData: ");
        printByte(sendData);
        // ---- test data end --- //


        // send
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        Debug.Log("send complete");
        totalRecvLen = 0;
        while (true)
        {
            recvData=new byte[2048];
            recvLen=serverSocket.Receive(recvData);
            if(recvLen==0)
            {
                con();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                totalRecvLen += recvLen;

                printByte(recvData);
                Debug.Log("recvLen: " + recvLen);
                merge();
                if (totalRecvLen >= 920*4 + 6){
                    Debug.Log("Wind Data Recv End");
                    break;
                }
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

        for (int i=0;i<23;i++){
            wind_speed[i] = new float[40];
            for (int j=0;j<40;j++){
                wind_speed[i][j] = BitConverter.ToSingle(recvTotal, idx); idx += 4;
                // Debug.Log("(" + i + " , " + j + " ): " + wind_speed[i][j]);
            }
        }
        Debug.Log("Parse End");
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
        parseData();
        SocketQuit();
    }
}