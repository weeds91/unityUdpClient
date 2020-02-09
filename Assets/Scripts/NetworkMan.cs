using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class NetworkMan : MonoBehaviour
{
    public PlayerController playerController;
    public CubeManager cubeManager;
    public Text playerList;
    public Text serverLog;
    public Text myIdText;
    public UdpClient udp;

    private String myId = "null";
    private bool playerAssigned = false;
    
    void Start()
    {
        udp = new UdpClient();
        
        udp.Connect("34.238.117.87",12345);

        Byte[] sendBytes = Encoding.ASCII.GetBytes("connect");
      
        udp.Send(sendBytes, sendBytes.Length);

        udp.BeginReceive(new AsyncCallback(OnReceived), udp);

        InvokeRepeating("HeartBeat", 1, 1);
    }

    void OnDestroy()
    {
        udp.Dispose();
    }

    public enum commands
    {
        NEW_CLIENT,
        UPDATE,
        OTHER_PLAYERS,
        PLAYER_LEFT,
        SELF
    };
    
    [Serializable]
    public class Message
    {
        public commands cmd;
        public Player[] players;
    }
    
    [Serializable]
    public class Player
    {
        public string id;
        public receivedColor color;
        public receivedPosition position;
        public receivedRotationEuler rotation;
    }

    [Serializable]
    public class receivedColor
    {
        public float R;
        public float G;
        public float B;
    }

    [Serializable]
    public class receivedPosition
    {
        public float X;
        public float Y;
        public float Z;
    }

    [Serializable]
    public class receivedRotationEuler
    {
        public float X;
        public float Y;
        public float Z;
    }

    [Serializable]
    public class GameState
    {
        public Player[] players;

        public void PrintPlayers(Text list)
        {
            string result = "Players:\n";
            for (int i = 0; i < players.Length; i++)
            {
                result += players[i].id + "\n";
            }
            list.text = result;
        }
    }

    public Message latestMessage;
    public GameState latestGameState;
    public Queue<Message> otherPlayersQueue = new Queue<Message>();
    public Queue<string> serverLogQueue = new Queue<string>();
    public Queue<Message> removePlayersQueue = new Queue<Message>();

    void OnReceived(IAsyncResult result)
    {
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;
        
        // points towards whoever had sent the message:
        IPEndPoint source = new IPEndPoint(0, 0);

        // get the actual message and fill out the source:
        byte[] message = socket.EndReceive(result, ref source);
        
        // do what you'd like with `message` here:
        string returnData = Encoding.ASCII.GetString(message);
        
        latestMessage = JsonUtility.FromJson<Message>(returnData);
        try
        {
            switch (latestMessage.cmd)
            {
                case commands.NEW_CLIENT:
                    serverLogQueue.Enqueue("Player " + latestMessage.players[0].id + " joined the server.");
                    break;
                case commands.UPDATE:
                    latestGameState = JsonUtility.FromJson<GameState>(returnData);
                    otherPlayersQueue.Enqueue(latestMessage);
                    break;
                case commands.OTHER_PLAYERS:
                    otherPlayersQueue.Enqueue(latestMessage);
                    break;
                case commands.PLAYER_LEFT:
                    serverLogQueue.Enqueue("Player " + latestMessage.players[0].id + " left the server.");
                    removePlayersQueue.Enqueue(latestMessage);
                    break;
                case commands.SELF:
                    myId = latestMessage.players[0].id;
                    Debug.Log("My ID: " + latestMessage.players[0].id);
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        
        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }

    void UpdatePlayerList()
    {
        while (otherPlayersQueue.Count > 0)
        {
            otherPlayersQueue.Dequeue();
            latestGameState.PrintPlayers(playerList);
            cubeManager.UpdateCubes(latestGameState.players);
        }
    }

    void UpdateServerLog()
    {
        while (serverLogQueue.Count > 0)
        {
            string str = serverLogQueue.Dequeue();
            serverLog.text += "\n" + str;
        }
    }

    void SpawnPlayers()
    {

    }

    void UpdatePlayers()
    {

    }

    void DestroyPlayers()
    {
        while (removePlayersQueue.Count > 0)
        {
            Message thisMessage = removePlayersQueue.Dequeue();
            cubeManager.RemoveCube(thisMessage.players[0]);
        }
    }
    
    void HeartBeat()
    {
        Byte[] sendBytes = Encoding.ASCII.GetBytes("heartbeat");
        udp.Send(sendBytes, sendBytes.Length);
    }

    public void SendPlayerPosition(Vector3 newPosition)
    {
        string posMessage = "{\"op\":\"cube_position\", \"position\":{\"X\":" + newPosition.x.ToString() + ", \"Y\":" + newPosition.y.ToString() + ",\"Z\":" + newPosition.z.ToString() + "}}";
        Byte[] sendBytes = Encoding.ASCII.GetBytes(posMessage);
        udp.Send(sendBytes, sendBytes.Length);
    }

    public void SendPlayerRotation(Vector3 newRotation)
    {
        string posMessage = "{\"or\":\"cube_rotation\", \"rotation\":{\"X\":" + newRotation.x.ToString() + ", \"Y\":" + newRotation.y.ToString() + ",\"Z\":" + newRotation.z.ToString() + "}}";
        Byte[] sendBytes = Encoding.ASCII.GetBytes(posMessage);
        udp.Send(sendBytes, sendBytes.Length);
    }

    private void UpdateMyId()
    {
        if (!playerAssigned && myId != "null")
        {
            myIdText.text = "My ID: " + myId;
            if (cubeManager.GetCube(myId))
            {
                playerController.SetPlayer(cubeManager.GetCube(myId));
                playerAssigned = true;
            }
        }
    }

    void Update()
    {
        UpdateServerLog();
        UpdatePlayerList();
        SpawnPlayers();
        UpdatePlayers();
        DestroyPlayers();
        UpdateMyId();
    }
}