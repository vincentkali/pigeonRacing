using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


public class MyClient : MonoBehaviour
{
    // ONLY ONE //
    public Socket serverSocket; 
    Thread connectThread;
    Thread updateThread;
    IPAddress ip;
    IPEndPoint ipEnd; 
    private int recvDataSize;

    // MULTIPLE //
    /// 1 ///
    byte[] recvData;
    byte[] sendData;
    int recvLen;
    int currentDataLen; // send used
    int currentParseLen; // recv used
    /// 2 ///
    byte[] recvData2;
    byte[] sendData2;
    int recvLen2;
    int currentDataLen2; // send used
    int currentParseLen2; // recv used

    // FLAG //
    public bool complete = false;
    public bool gameUpdate = false;
    public bool canUpdate = false;

    // RECEIVE OBJ //
    public S2C_LOGIN S2C_LOGIN_obj;
    public S2C_PLAYER_PIGEON S2C_PLAYER_PIGEON_obj;
    public S2C_PLAYER_DATA S2C_PLAYER_DATA_obj;
    public S2C_GAME_LIST S2C_GAME_LIST_obj;
    public S2C_GAME_UPDATE S2C_GAME_UPDATE_obj;
    public S2C_PIGEON_ACTION S2C_PIGEON_ACTION_obj;
    int npigeonGlo;

    void initValue()
    {
        ip = IPAddress.Parse("59.124.3.23");
        recvDataSize = 10000;
        ipEnd = new IPEndPoint(IPAddress.Parse("59.124.3.23"),2919);
        recvData=new byte[recvDataSize];
        sendData=new byte[1024];
        S2C_LOGIN_obj = new S2C_LOGIN();
        S2C_PLAYER_PIGEON_obj = new S2C_PLAYER_PIGEON();
        S2C_PLAYER_DATA_obj = new S2C_PLAYER_DATA();
        S2C_GAME_LIST_obj = new S2C_GAME_LIST();
        S2C_GAME_UPDATE_obj = new S2C_GAME_UPDATE();
        S2C_PIGEON_ACTION_obj = new S2C_PIGEON_ACTION();
    }
    void connection()
    {
        if(serverSocket!=null){
            // serverSocket.Close();
            Debug.Log("Has Socket");
        }else{
            initValue();
            Debug.Log("Create New socket");
            serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            serverSocket.Connect(ipEnd);
        }
        
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
    }
    public void printByte(byte[] b)
    {
        Debug.Log(BitConverter.ToString(b));
    }
    public void C2S_PIGEON_ACTION(int pid_, int rkey_, long action_, long[] idpigeon_, int npigeon_) // npigeon_ self add
    {
        complete = false;
        connectThread=new Thread(() => C2S_PIGEON_ACTION_Thread(pid_, rkey_, action_, idpigeon_, npigeon_));
        connectThread.Start();
    }
    void C2S_PIGEON_ACTION_Thread(int pid_, int rkey_, long action_, long[] idpigeon_, int npigeon_)
    {
        // Debug.Log("C2S_PIGEON_ACTION_Thread");
        connection();
        npigeonGlo = npigeon_;
        int headerLen = 3;
        ushort sendDataSize = Convert.ToUInt16(4+4+8+8*npigeon_);
        sendData=new byte[headerLen + 2 + sendDataSize];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x40;

        currentDataLen = headerLen;
        addUShort(sendDataSize);
        addInt(pid_);
        addInt(rkey_);
        addLong(action_);
        for (int i=0; i<npigeon_; i++){
            addLong(idpigeon_[i]);
        }
        
        // Debug.Log("sendData: ");
        // printByte(sendData);
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        // while (true)
        // {
        //     recvData=new byte[recvDataSize];
        //     recvLen=serverSocket.Receive(recvData);
        //     Debug.Log("C2S_PIGEON_ACTION recvData: ");
        //     printByte(recvData);
        //     if(recvLen==0)
        //     {
        //         // connection();
        //         Debug.Log("recv 0 data");
        //         // continue;
        //     } else {
        //         currentParseLen=0;
        //         byte mark = readByte();
        //         byte version = readByte();
        //         byte command = readByte();

        //         currentDataLen2 = currentDataLen;
        //         recvData2 = recvData;
        //         if (command == 0x40){
        //             C2S_PIGEON_ACTION_Parse(npigeon_);
        //         }else if (command == 0x61){
        //             S2C_GAME_UPDATE_Parse();
        //         }
        //         // todo // 
                
        //         complete = true;
        //         break;
        //     }
        // }
    }
    void C2S_PIGEON_ACTION_Parse(int npigeon_)
    {
        Debug.Log("C2S_PIGEON_ACTION_Parse");
        ushort dataSiza = readUShort();
        S2C_PIGEON_ACTION_obj.raw = recvData;
        S2C_PIGEON_ACTION_obj.action = readLong();
        S2C_PIGEON_ACTION_obj.idpigeon = new long[npigeon_];
        for(int i=0; i<npigeon_; i++){
            S2C_PIGEON_ACTION_obj.idpigeon[i] = readLong();
        }
        S2C_PIGEON_ACTION_obj.npigeon = npigeon_;
    }

    public void S2C_GAME_UPDATE()
    {
        gameUpdate = false;
        updateThread=new Thread(() => S2C_GAME_UPDATE_Thread());
        updateThread.Start();
    }
    void S2C_GAME_UPDATE_Thread()
    {
        connection();
        while (true)
        {
            // if (canUpdate == false){
            //     Debug.Log("can not update");
            //     continue;
            // }
            recvData2=new byte[recvDataSize];
            // Debug.Log("before");
            recvLen2=serverSocket.Receive(recvData2);
            Debug.Log("S2C_GAME_UPDATE_Thread recvData: ");
            printByte(recvData2);
            if(recvLen2==0)
            {
                // connection();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                
                currentParseLen2=0;
                byte mark = readByte(2);
                byte version = readByte(2);
                byte command = readByte(2);

                currentDataLen = currentParseLen2;
                recvData = recvData2;
                if (command == 0x40){
                    C2S_PIGEON_ACTION_Parse(npigeonGlo);
                }else if (command == 0x61){
                    S2C_GAME_UPDATE_Parse();
                }
                // todo //


                // S2C_GAME_UPDATE_Parse();
                // Debug.Log("=== update ===");
                // break;
            }
        }
    }
    void S2C_GAME_UPDATE_Parse()
    {
        Debug.Log("S2C_GAME_UPDATE_Parse");
        ushort dataSize = readUShort(2);
        S2C_GAME_UPDATE_obj.raw = recvData2;
        S2C_GAME_UPDATE_obj.sn = readLong(2);
        S2C_GAME_UPDATE_obj.npigeon = readShort(2);
        S2C_GAME_UPDATE_obj.status = readByte(2);
        S2C_GAME_UPDATE_obj.time = readInt(2);

        gameUpdate = true;
        // canUpdate = false;
        complete = true;
    }
    public void C2S_GAME_LIST(int pid_, int rkey_)
    {
        complete = false;
        connectThread=new Thread(() => C2S_GAME_LIST_Thread(pid_, rkey_));
        connectThread.Start();
    }
    void C2S_GAME_LIST_Thread(int pid_, int rkey_)
    {
        connection();
        int headerLen = 3;
        ushort sendDataSize = Convert.ToUInt16(8);
        sendData=new byte[headerLen + 2 + sendDataSize];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x60;

        currentDataLen = headerLen;
        addUShort(sendDataSize);
        addInt(pid_);
        addInt(rkey_);

        // Debug.Log("C2S_GAME_LIST_Thread sendData: ");
        // printByte(sendData);
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        while (true)
        {
            recvData=new byte[recvDataSize];
            recvLen=serverSocket.Receive(recvData);
            Debug.Log("C2S_GAME_LIST_Thread recvData: ");
            printByte(recvData);
            if(recvLen==0)
            {
                // connection();
                Debug.Log("recv 0 data");
                // continue;
            } else {
                currentParseLen=0;
                byte mark = readByte();
                byte version = readByte();
                byte command = readByte();
                C2S_GAME_LIST_Parse();
                complete = true;
                break;
            }
        }
    }
    void C2S_GAME_LIST_Parse()
    {
        ushort dataSize = readUShort();
        S2C_GAME_LIST_obj.raw = recvData;

        int oneDataLen = 8 + 1 + 2 + 2 + 1 + 2 + 1 + 4;
        Debug.Log($"dataSize: {dataSize}");
        int N = dataSize / oneDataLen;
        S2C_GAME_LIST_obj.ngame = N;

        S2C_GAME_LIST_obj.gamedata_list = new gameData[N];
        for (int i=0;i<N;i++){
            S2C_GAME_LIST_obj.gamedata_list[i].sn = readLong();
            S2C_GAME_LIST_obj.gamedata_list[i].kind = readByte();
            S2C_GAME_LIST_obj.gamedata_list[i].idxdata = readShort();
            S2C_GAME_LIST_obj.gamedata_list[i].id = readShort();
            S2C_GAME_LIST_obj.gamedata_list[i].type = readByte();
            S2C_GAME_LIST_obj.gamedata_list[i].npigeon = readShort();
            S2C_GAME_LIST_obj.gamedata_list[i].status = readByte();
            S2C_GAME_LIST_obj.gamedata_list[i].time = readInt();
        }
    }
    public void C2S_PLAYER_DATA(int pid_, int rkey_)
    {
        complete = false;
        connectThread=new Thread(() => C2S_PLAYER_DATA_Thread(pid_, rkey_));
        connectThread.Start();
    }
    void C2S_PLAYER_DATA_Thread(int pid_, int rkey_)
    {
        connection();

        int headerLen = 3;
        ushort sendDataSize = Convert.ToUInt16(8);
        sendData=new byte[headerLen + 2 + sendDataSize];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x10;

        currentDataLen = headerLen;
        addUShort(sendDataSize);
        addInt(pid_);
        addInt(rkey_);
        // Debug.Log("sendData: ");
        // printByte(sendData);
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        while (true)
        {
            recvData=new byte[recvDataSize];
            recvLen=serverSocket.Receive(recvData);
            // Debug.Log("recvData: ");
            // printByte(recvData);
            if(recvLen==0)
            {
                // connection();
                Debug.Log("recv 0 data");
                // continue;
            } else {
                currentParseLen=0;
                byte mark = readByte();
                byte version = readByte();
                byte command = readByte();
                C2S_PLAYER_DATA_Parse();

                complete = true;
                break;
            }
        }
    }
    void C2S_PLAYER_DATA_Parse()
    {
        ushort dataSiza = readUShort();
        S2C_PLAYER_DATA_obj.raw = recvData;
        int coin = S2C_PLAYER_DATA_obj.coin = readInt();
    }
    public void C2S_LOGIN_PLAY(string token_, string nickname_){
        complete = false;
        connectThread=new Thread(() => C2S_LOGIN_PLAY_Thread(token_, nickname_));
        connectThread.Start();
    }
    void C2S_LOGIN_PLAY_Thread(string token_, string nickname_){
        connection();

        int headerLen = 3;
        byte[] sendDataBuffer=new byte[1000];
        sendDataBuffer[0] = 0xc8;
        sendDataBuffer[1] = 0x01;
        sendDataBuffer[2] = 0x07;

        currentDataLen = headerLen;
        currentDataLen += 2;

        byte[] byteBuffer = Encoding.UTF8.GetBytes(token_);
        Buffer.BlockCopy(byteBuffer, 0, sendDataBuffer, currentDataLen, byteBuffer.Length);
        currentDataLen += byteBuffer.Length;
        // Debug.Log($"byteBuffer.Length: {byteBuffer.Length}");

        byteBuffer = new byte[1];
        byteBuffer[0] = 0x09;
        Buffer.BlockCopy(byteBuffer, 0, sendDataBuffer, currentDataLen, 1);
        currentDataLen += 1;

        byteBuffer = Encoding.UTF8.GetBytes(nickname_);
        Buffer.BlockCopy(byteBuffer, 0, sendDataBuffer, currentDataLen, byteBuffer.Length);
        currentDataLen += byteBuffer.Length;

        int payloadLen = currentDataLen - 5;
        ushort[] ushortBuffer = new ushort[1];
        ushortBuffer[0] = Convert.ToUInt16(payloadLen);
        Buffer.BlockCopy(ushortBuffer, 0, sendDataBuffer, headerLen, 2); 
        
        sendData = new byte[currentDataLen];
        Buffer.BlockCopy(sendDataBuffer, 0, sendData, 0, currentDataLen);
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        // Debug.Log("sendData: ");
        // printByte(sendData);

        while (true)
        {
            recvData=new byte[recvDataSize];
            recvLen=serverSocket.Receive(recvData);
            // Debug.Log("recvData: ");
            // printByte(recvData);
            if(recvLen==0)
            {
                // connection();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                int currentParseLen=0;
                byte mark = readByte();
                byte version = readByte();
                byte command = readByte();
                C2S_LOGIN_PLAY_Parse(currentParseLen);
                
                complete = true;
                break;
            }
        }
    }
    void C2S_LOGIN_PLAY_Parse(int currentParseLen)
    {
        ushort dataSiza = readUShort();
        S2C_LOGIN_obj.pid = readInt();
        // Debug.Log($"recv pid: {S2C_LOGIN_obj.pid}");
        S2C_LOGIN_obj.rkey = readInt();
        // Debug.Log($"recv rkey: {S2C_LOGIN_obj.rkey}");

        byte[] byteBuffer = new byte[300];
        Buffer.BlockCopy(recvData, currentParseLen, byteBuffer, 0, 300);
        S2C_LOGIN_obj.nickname = Encoding.UTF8.GetString(byteBuffer);
        // Debug.Log($"recv name: {S2C_LOGIN_obj.nickname}");
        S2C_LOGIN_obj.raw = recvData;
    }
    
    public void C2S_PLAYER_PIGEON(int pid_, int rkey_){
        complete = false;
        connectThread=new Thread(() => C2S_PLAYER_PIGEON_Thread(pid_, rkey_));
        connectThread.Start();
    }
    void C2S_PLAYER_PIGEON_Thread(int pid_, int rkey_){
        connection();
        int headerLen = 3;
        ushort sendDataSize = Convert.ToUInt16(8);
        sendData=new byte[headerLen + 2 + sendDataSize];
        sendData[0] = 0xc8;
        sendData[1] = 0x01;
        sendData[2] = 0x20;

        currentDataLen = headerLen;
        addUShort(sendDataSize);
        addInt(pid_);
        addInt(rkey_);

        // Debug.Log("sendData: ");
        // printByte(sendData);
        serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
        while (true)
        {
            recvData=new byte[recvDataSize];
            recvLen=serverSocket.Receive(recvData);
            // Debug.Log("recvData: ");
            // printByte(recvData);
            if(recvLen==0)
            {
                // connection();
                Debug.Log("recv 0 data");
                // continue;
                
            } else {
                currentParseLen=0;
                byte mark = readByte();
                byte version = readByte();
                byte command = readByte();
                C2S_PLAYER_PIGEON_Parse();
                
                complete = true;
                break;
            }
        }
    }
    void C2S_PLAYER_PIGEON_Parse()
    {
        ushort dataSize = readUShort();
        S2C_PLAYER_PIGEON_obj.raw = recvData;
        int N = S2C_PLAYER_PIGEON_obj.npigeon = readInt();

        S2C_PLAYER_PIGEON_obj.pigeonAtt_list = new pigeonAtt[N];
        for (int i=0;i<N;i++){
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].id = readLong();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].intelligence = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].vision = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].body = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].feather = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].endurance = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].muscle = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].stomach = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].waterproof = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].fatigue = readFloat();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].sngame = readLong();
            S2C_PLAYER_PIGEON_obj.pigeonAtt_list[i].age = readShort();
        }
        
        S2C_PLAYER_PIGEON_obj.pigeonName_list = new String[N];
        int len;
        for (int i=0;i<N;i++){
            S2C_PLAYER_PIGEON_obj.pigeonName_list[i] = getName(recvData, currentParseLen, out len);
            currentParseLen += len + 1;
        }
    }






    
    void addUShort(ushort data)
    {
        ushort[] ushortBuffer = new ushort[1];
        ushortBuffer[0] = data;
        Buffer.BlockCopy(ushortBuffer, 0, sendData, currentDataLen, sizeof(ushort) * 1); 
        currentDataLen += sizeof(ushort);
    }
    void addInt(int data)
    {
        int[] intBuffer = new int[1];
        intBuffer[0] = data;
        Buffer.BlockCopy(intBuffer, 0, sendData, currentDataLen, 4);
        currentDataLen += 4;
    }
    void addLong(long data)
    {
        long[] longBuffer = new long[1];
        longBuffer[0] = data;
        Buffer.BlockCopy(longBuffer, 0, sendData, currentDataLen, 8);
        currentDataLen += 8;
    }
    byte readByte(int opt = 1)
    {
        byte data;
        if (opt == 1){
            data = recvData[currentParseLen];
            currentParseLen += sizeof(byte);
        }else{
            data = recvData2[currentParseLen2];
            currentParseLen2 += sizeof(byte);
        }
        
        return data;
    }
    ushort readUShort(int opt = 1)
    {
        ushort data;
        if (opt == 1){
            data = BitConverter.ToUInt16(recvData, currentParseLen);
            currentParseLen += sizeof(ushort);
        }else{
            data = BitConverter.ToUInt16(recvData2, currentParseLen2);
            currentParseLen2 += sizeof(ushort);
        }

        return data;
    }
    int readInt(int opt = 1)
    {
        int data;
        if (opt == 1){
            data = BitConverter.ToInt32(recvData, currentParseLen);
            currentParseLen += sizeof(int);
        }else{
            data = BitConverter.ToInt32(recvData2, currentParseLen2);
            currentParseLen2 += sizeof(int);
        }
        
        return data;
    }
    long readLong(int opt = 1)
    {
        long data;
        if (opt == 1){
            data = BitConverter.ToInt64(recvData, currentParseLen);
            currentParseLen += sizeof(long);
        }else{
            data = BitConverter.ToInt64(recvData2, currentParseLen2);
            currentParseLen2 += sizeof(long);
        }
        
        return data;
    }
    float readFloat(int opt = 1)
    {
        float data;
        if (opt == 1){
            data = BitConverter.ToSingle(recvData, currentParseLen);
            currentParseLen += sizeof(float);
        }else{
            data = BitConverter.ToSingle(recvData2, currentParseLen2);
            currentParseLen2 += sizeof(float);
        }

        return data;
    }
    short readShort(int opt = 1)
    {
        short data;
        if (opt == 1){
            data = BitConverter.ToInt16(recvData, currentParseLen);
            currentParseLen += sizeof(short);
        }else{
            data = BitConverter.ToInt16(recvData2, currentParseLen2);
            currentParseLen2 += sizeof(short);
        }

        return data;
    }
    string getName(byte[] recvData_, int currentParseLen_, out int len_){
        len_=0;
        while (true){
            if (recvData_[currentParseLen_ + len_] == 0x09){
                break;
            }else{
                len_++;
            }
        }

        byte[] buf = new byte[1000];
        Buffer.BlockCopy(recvData_, currentParseLen_, buf, 0, len_);
        string name = Encoding.UTF8.GetString(buf);

        // Debug.Log($"Name: {name}");
        
        return name;
    }
    // void merge()
    // {
    //     // Merge into recvTotal[]
    //     Buffer.BlockCopy(recvData, 0, recvTotal, mergeIdx, recvLen);
    //     mergeIdx += recvLen;
    // }
}
