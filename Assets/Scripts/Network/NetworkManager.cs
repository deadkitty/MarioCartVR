//using UnityEngine;
//using System.Collections;
//using System;

//public class NetworkManager : MonoBehaviour
//{
//    private static NetworkManager sInstance;

//    private String typeName = "SuperVirtualCartGame";
    
//    public GameObject playerPrefab;
//    public GameObject[] spawns;

//    private GameObject playerCart;

//    private HostData[] hostList;
//    private int playersConnected = 0;
//    private int playersWhoWantsToReset = 0;
//    private int playerID = -1;

//    private bool allPlayersConnected = false;


//    public static HostData[] HostList
//    {
//        get { return sInstance.hostList; }
//    }
        
//    public static int PlayersConnected
//    {
//        get { return sInstance.playersConnected; }
//        set
//        {
//            sInstance.playersConnected = value;
//            CheckAllPlayersConnected();
//        }
//    }

//    public static int PlayerID
//    {
//        get { return sInstance.playerID; }
//    }
    
//    void Start()
//    {
//        sInstance = this;
//    }

    
//    public static void StartServer()
//    {
//        Network.InitializeServer(Players.playerCount - 1, 25000, !Network.HavePublicAddress());
//        MasterServer.RegisterHost(sInstance.typeName, Startup.GameName);
//    }

//    public static void StopServer()
//    {
//        Network.Disconnect();
//        MasterServer.UnregisterHost();
//    }

//    public static void RefreshHostList()
//    {
//        MasterServer.RequestHostList(sInstance.typeName);
//    }

//    public static void JoinServer(HostData host)
//    {
//        Debug.Log("Join Server " + host.gameName);
//        Network.Connect(host);
//    }

//    public static void LeaveServer()
//    {
//        Network.Disconnect();
//    }

//    public static void Reset()
//    {
//        if(Network.isServer)
//        {
//            sInstance.PlayerWantsToReset();
//        }
//        else
//        {
//            sInstance.networkView.RPC("PlayerWantsToReset", RPCMode.Server);
//        }
//    }

//    private static void CheckAllPlayersConnected()
//    {
//        Debug.Log(PlayersConnected + " from " + Players.playerCount + " connected");

//        if (PlayersConnected == Players.playerCount)
//        {
//            sInstance.networkView.RPC("StartRace", RPCMode.AllBuffered);
//        }
//    }

//    private static void SpawnPlayer()
//    {
//        Transform spawn = sInstance.spawns[PlayerID].transform;
//        sInstance.playerCart = Network.Instantiate(sInstance.playerPrefab, spawn.position, spawn.rotation, 0) as GameObject;
//    }

//    void OnMasterServerEvent(MasterServerEvent msEvent)
//    {
//        if (msEvent == MasterServerEvent.HostListReceived)
//        {
//            Debug.Log("Hostlist Received");
//            hostList = MasterServer.PollHostList();
            
//            Debug.Log("Lengt: " + hostList.Length);

//            if(hostList.Length == 0)
//            {
//                Debug.Log("No Server Initialized");
//                MenuController.ShowMenu(Menu.EType.main);
//            }
//            else
//            {
//                foreach (HostData data in hostList)
//                {
//                    Debug.Log("Name : " + data.gameName);
//                    Debug.Log("Limit: " + data.playerLimit);
//                    Debug.Log("Connected: " + data.connectedPlayers);

//                    if (data.gameName == Startup.GameName && data.connectedPlayers < data.playerLimit)
//                    {
//                        JoinServer(data);
//                    }
//                }
//            }
//        }
//    }

//    void OnServerInitialized()
//    {
//        Debug.Log("Server Initializied");
        
//        SetPlayerID(playersConnected);
        
//        LoadLevel();
//    }

//    void OnPlayerConnected(NetworkPlayer player)
//    {
//        Debug.Log("Player " + playersConnected + " connected from " + player.ipAddress + ":" + player.port);

//        networkView.RPC("SetPlayerID", player, playersConnected);
//    }

//    void OnPlayerDisconnected(NetworkPlayer player)
//    {
//        Network.RemoveRPCs(player);
//        Network.DestroyPlayerObjects(player);

//        --PlayersConnected;
//    }

//    void OnConnectedToServer()
//    {
//        Debug.Log("Connected to server");
//        LoadLevel();
//    }



//    [RPC]
//    void SetPlayerID(int id)
//    {
//        Debug.Log("SetPlayerID");

//        playerID = id;
//    }

//    [RPC]
//    void StartRace()
//    {
//        SpawnPlayer();
//        CartTimer.StartRaceTimer();
//    }

//    [RPC]
//    void PlayerWantsToReset()
//    {
//        ++playersWhoWantsToReset;

//        if(playersWhoWantsToReset == playersConnected)
//        {
//            sInstance.networkView.RPC("ResetPlayer", RPCMode.AllBuffered);
//        }
//    }

//    [RPC]
//    void ResetPlayer()
//    {
//        if(networkView.isMine)
//        {
//            playerCart.transform.position = spawns[playerID].transform.position;
//            playerCart.transform.rotation = spawns[playerID].transform.rotation;

//            playerCart.GetComponent<ItemController>().currentItem = Item.EItemType.none;

//            playersWhoWantsToReset = 0;

//            CartTimer.StartRaceTimer();
//        }
//    }

//    [RPC]
//    void Ready()
//    {
//        Debug.Log("Player is Ready");
//        ++PlayersConnected;
//    }
//}

using UnityEngine;
using System;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    #region Fields

    private static NetworkManager sInstance;

    private String typeName = "SuperVirtualCartGame";

    public GameObject playerPrefab;
    public GameObject[] spawns;

    private GameObject playerCart;

    private static int playerIndex = 0;
    
    private static int playersWhoWantsToReset = 0;

    #endregion

    #region Properties

    //public static HostData[] HostList
    //{
    //    get { return sInstance.hostList; }
    //}

    //public static int PlayersConnected
    //{
    //    get { return sInstance.playersConnected; }
    //    set { sInstance.playersConnected = value; }
    //}

    public static int PlayerIndex
    {
        get { return playerIndex; }
    }

    #endregion
    
    #region Events

    void Start()
    {
        sInstance = this;
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            Debug.Log("Hostlist Received");

            HostData[] hostList = MasterServer.PollHostList();

            bool joinServer = false;
            foreach (HostData data in hostList)
            {
                if (data.gameName == Startup.GameName && data.connectedPlayers < data.playerLimit)
                {
                    JoinServer(data);
                    joinServer = true;
                }
            }

            if (!joinServer)
            {
                Debug.Log("No Free Server Available!");
                MenuController.ShowMenu(Menu.EType.main);
            }
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        Debug.Log("Player " + Network.connections.Length + " connected from " + Network.player.ipAddress + ":" + Network.player.port + "(Server)");

        SetPlayerIndex(Network.connections.Length);

        Startup.LoadLevel();
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + Network.connections.Length + " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
        
        Startup.LoadLevel();
    }

    #endregion

    #region Class Functions

    public static void StartServer()
    {
        Network.InitializeServer(Players.playerCount - 1, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(sInstance.typeName, Startup.GameName);
    }

    public static void StopServer()
    {
        Network.Disconnect();
        MasterServer.UnregisterHost();
    }

    public static void RefreshHostList()
    {
        MasterServer.RequestHostList(sInstance.typeName);
    }

    public static void JoinServer(HostData host)
    {
        Debug.Log("Join Server " + host.gameName);
        Network.Connect(host);
    }

    public static void LeaveServer()
    {
        Network.Disconnect();
    }

    public static void Reset()
    {
        if (Network.isServer)
        {
            sInstance.PlayerWantsToReset();
        }
        else
        {
            sInstance.networkView.RPC("PlayerWantsToReset", RPCMode.Server);
        }
    }

    public static void PlayerReady()
    {
        if (Network.isServer)
        {
            sInstance.Ready();
        }
        else
        {
            sInstance.networkView.RPC("Ready", RPCMode.Server);
        }
    }

    private static void SpawnPlayer()
    {
        Transform spawn = sInstance.spawns[playerIndex].transform;
        sInstance.playerCart = Network.Instantiate(sInstance.playerPrefab, spawn.position, spawn.rotation, 0) as GameObject;
    }

    #endregion

    #region RPC Functions

    [RPC]
    void SetPlayerIndex(int index)
    {
        Debug.Log("SetPlayerIndex " + index);

        playerIndex = index;
    }

    [RPC]
    void StartRace(int index)
    {
        Debug.Log("SetPlayerIndex " + index);

        playerIndex = index;

        Debug.Log("StartRace");

        SpawnPlayer();
        CartTimer.StartRaceTimer();
    }

    [RPC]
    void PlayerWantsToReset()
    {
        ++playersWhoWantsToReset;

        if (playersWhoWantsToReset == Network.maxConnections + 1)
        {
            sInstance.networkView.RPC("ResetPlayer", RPCMode.AllBuffered);
        }
    }

    [RPC]
    void ResetPlayer()
    {
        if (networkView.isMine)
        {
            playerCart.transform.position = spawns[playerIndex].transform.position;
            playerCart.transform.rotation = spawns[playerIndex].transform.rotation;

            playerCart.GetComponent<ItemController>().currentItem = Item.EItemType.none;

            playersWhoWantsToReset = 0;

            CartTimer.StartRaceTimer();
        }
    }

    [RPC]
    void Ready()
    {
        Debug.Log("Player is Ready");
        
        if(Network.connections.Length == Network.maxConnections)
        {
            for (int i = 0; i < Network.connections.Length; ++i )
            {
                networkView.RPC("StartRace", Network.connections[i], i + 1);
            }

            StartRace(0);
        }
    }

    #endregion

    #region Member Functions

    #endregion
}