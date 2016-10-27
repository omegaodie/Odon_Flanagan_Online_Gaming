using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine.UI;

public class GameScript : MonoBehaviour {

    public Sprite plyrOne; // Drag your first sprite here
    public Transform tOne;
    public Sprite plyrTwo; // Drag your first sprite here
    public Transform tTwo;

    string PlayerOneIP = "149.153.106.160";
    int portA = 8001;

    string PlayerTwoIP = "149.153.106.176";
    int portB = 8002;

    string instancePlayer;
    bool isState;


    int socketPlayerOne;
    int socketPlayerTwo;

    int myReiliableChannelId;
    int connectionID;
    ConnectionConfig config;
    HostTopology topology;


   

    


    // Use this for initialization
    void Start () {
        isState = false;
        instancePlayer = "";
        NetworkTransport.Init();
        config = new ConnectionConfig();
        myReiliableChannelId = config.AddChannel(QosType.Reliable);
        topology = new HostTopology(config, 10);
    }

    // Update is called once per frame
    void Update () {
        getKeyInput();
        recieve();
	}

    public void changeInputTypeRemote()
    {
        if (isState == false)
        {
            isState = true;
        }
        else
        {
            isState = false;
        }
    }

    public void changeInputTypeLocal()
    {
        if (isState == false)
        {
            isState = true;
        }
        else
        {
            isState = false;
        }
        send("Switch");
    }


    public void send(string s)
    {

        byte error = 0;
        byte[] buffer = new byte[100];
        System.IO.Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, s);
        if (instancePlayer == "p1")
        {
            NetworkTransport.Send(socketPlayerOne, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }
        else if (instancePlayer == "p2")
        {
            NetworkTransport.Send(socketPlayerTwo, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }


    }

    void getKeyInput()
    {
        if (instancePlayer == "p1")
        {
            if (Input.GetKeyDown("w"))
            {

                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(tOne.position.x, 15, 100), .4f);
                if(isState == true)
                {
                    send(SerializeVector3Array(tOne.position));
                }
                else
                {
                    send("w");
                }
                
            }
            if (Input.GetKeyDown("a"))
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(15, tOne.position.y, 100), .4f);
                if (isState == true)
                {
                    send(SerializeVector3Array(tOne.position));
                }
                else
                {
                    send("a");
                }
                
            }
            if (Input.GetKeyDown("s"))
            {

                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(tOne.position.x, -15, 100), .4f);
                if (isState == true)
                {
                    send(SerializeVector3Array(tOne.position));
                }
                else
                {
                    send("s");
                }
                
            }
            if (Input.GetKeyDown("d"))
            {

                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(-15, tOne.position.y, 100), .4f);
                if (isState == true)
                {
                    send(SerializeVector3Array(tOne.position));
                }
                else
                {
                    send("d");
                }
                
            }
        }
        else if (instancePlayer == "p2")
        {
                if (Input.GetKeyDown("w"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(tTwo.position.x, 15, 100), .4f);
                        if (isState == true)
                        {
                            send(SerializeVector3Array(tTwo.position));
                        }
                        else
                        {
                            send("w");
                        }
                
                }
                if (Input.GetKeyDown("a"))
                {
                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(15, tTwo.position.y, 100), .4f);
                        if (isState == true)
                        {
                              send(SerializeVector3Array(tTwo.position));
                        }
                        else
                        {
                            send("a");
                        }

                    }
                if (Input.GetKeyDown("s"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(tTwo.position.x, -15, 100), .4f);
                        if (isState == true)
                        {
                             send(SerializeVector3Array(tTwo.position));
                        }
                        else
                        {
                         send("s");
                        }
                
                }
                if (Input.GetKeyDown("d"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(-15, tTwo.position.y, 100), .4f);
                        if (isState == true)
                        {
                            send(SerializeVector3Array(tTwo.position));
                        }
                        else
                        {
                            send("d");
                        }
                
                }
            }
        }

    public void setupA()
    {
        socketPlayerOne = NetworkTransport.AddHost(topology, portA);
        instancePlayer = "p1";
    }
    public void setupB()
    {
       socketPlayerTwo = NetworkTransport.AddHost(topology, portB);
       instancePlayer = "p2";
    }

    public void join()
    {
        if (instancePlayer == "p1")
        {
            byte error = 0;
            connectionID = NetworkTransport.Connect(socketPlayerTwo, PlayerTwoIP, portB, 0, out error);
        }
        else if (instancePlayer == "p2")
        {
            byte error = 0;
            connectionID = NetworkTransport.Connect(socketPlayerOne, PlayerOneIP, portA, 0, out error);
        }
    }


    private static string SerializeVector3Array(Vector3 aVector)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(aVector.x).Append(" ").Append(aVector.y).Append(" ").Append(aVector.z).Append("|");

        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }



    private static Vector3 DeserializeVector3Array(string aData)
    {

        Vector3 result = new Vector3();

        string[] values = aData.Split(' ');
        if (values.Length != 3)
        {
            throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
        }
        result = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        return result;
    }

    public void inputbasedMovement(string c)
    {
        if (instancePlayer == "p1")
        {

            if (c == "w")
            {
                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);

            }
            if (c == "a")
            {
                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(15, 3, 100), .4f);

            }
            if (c == "s")
            {

                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);

            }
            if (c == "d")
            {

                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
            }
        }
        else if (instancePlayer == "p2")
        {
            if (c == "w")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
            }
            if (c == "a")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(15, 3, 100), .4f);
            }
            if (c == "s")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
            }
            if (c == "d")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
            }
        }
    }

    public void recieve()
    {
        int remoteSocketId, remoteConnectionId, remoteChannelId, bufferSize = 500, dataSize;
        string dataIn;
        byte[] recBuffer = new byte[500];
        byte error = 0;
        NetworkEventType receivedData = NetworkTransport.Receive(out remoteSocketId, out remoteConnectionId, out remoteChannelId, recBuffer, bufferSize, out dataSize, out error);
        switch (receivedData)
        {
                    case NetworkEventType.Nothing:         //1
                        break;
                    case NetworkEventType.ConnectEvent:    //2
                        Debug.Log("Connect event");

                        break;
                    case NetworkEventType.DataEvent:       //3

                        Stream stream = new MemoryStream(recBuffer);
                        BinaryFormatter formatter = new BinaryFormatter();
                        dataIn = formatter.Deserialize(stream) as string;
                        if(dataIn == "Switch")
                        {
                            Debug.Log("switch");
                            changeInputTypeRemote();
                        }
                        else if(isState == true)
                        {
                            Debug.Log(dataIn);
                            if (instancePlayer == "p1")
                            {
                                tTwo.position = DeserializeVector3Array(dataIn);
                            }
                            else if (instancePlayer == "p2")
                            {
                                tOne.position = DeserializeVector3Array(dataIn);
                            }
                        }
                        else
                        {
                            inputbasedMovement(dataIn);
                        }
                        break;

                    case NetworkEventType.DisconnectEvent: //4
                        break;
            }
        }
}
