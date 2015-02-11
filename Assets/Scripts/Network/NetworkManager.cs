using UnityEngine;
using System;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    #region Fields

    private static NetworkManager sInstance;

    private const String typeName = "SuperVirtualCartGame";

    public GameObject playerPrefab;
    private GameObject playerCart;

    private static int playerIndex = -1;    
    private static int playersWhoWantToReset = 0;

//#if UNITY_EDITOR

    public bool startOffline = false;

//#endif

    #endregion

    #region Properties

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
                if (data.gameName == App.GameName && data.connectedPlayers < data.playerLimit)
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

        App.LoadLevel(App.Level);

        SetPlayerIndex(Network.connections.Length);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + Network.connections.Length + " connected from " + player.ipAddress + ":" + player.port);

        networkView.RPC("SetPlayerIndex", player, Network.connections.Length);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");

        App.LoadLevel(App.Level);
    }

    #endregion

    #region Class Functions

    public static void StartServer()
    {
        Network.InitializeServer(App.MaxPlayers - 1, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, App.GameName);
    }

    public static void StopServer()
    {
        for (int i = 0; i < App.MaxPlayers; ++i)
        {
            Network.Destroy(Players.GetPlayer(i));
        }

        Network.Disconnect();
        MasterServer.UnregisterHost();
    }

    public static void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
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
        Transform spawn = App.Spawns[playerIndex].transform;
        sInstance.playerCart = Network.Instantiate(sInstance.playerPrefab, spawn.position, spawn.rotation, 0) as GameObject;
    }

    #endregion

    #region RPC Functions

    [RPC]
    void SetPlayerIndex(int index)
    {
        Debug.Log("SetPlayerIndex " + index);

        playerIndex = index;

        PlayerReady();
    }

    [RPC]
    void StartRace()
    {
        Debug.Log("StartRace");

        SpawnPlayer();
        CartTimer.StartRaceTimer();
    }

    [RPC]
    void PlayerWantsToReset()
    {
        ++playersWhoWantToReset;

        if (playersWhoWantToReset == Network.maxConnections + 1)
        {
            sInstance.networkView.RPC("ResetPlayer", RPCMode.AllBuffered);
        }
    }

    [RPC]
    void ResetPlayer()
    {
        if (networkView.isMine)
        {
            playerCart.transform.position = App.Spawns[playerIndex].transform.position;
            playerCart.transform.rotation = App.Spawns[playerIndex].transform.rotation;

            playerCart.GetComponent<ItemController>().currentItem = Item.EItemType.none;

            playersWhoWantToReset = 0;

            CartTimer.Reset();
            CartTimer.StartRaceTimer();
        }
    }

    [RPC]
    void Ready()
    {
        Debug.Log("Player is Ready");
        Debug.Log("Connections: " + Network.connections.Length + " MaxConnections: " + Network.maxConnections);

//#if UNITY_EDITOR

        if (startOffline)
        {
            StartRace();
        }

        if (!startOffline)
        {

//#endif

            if (Network.connections.Length == Network.maxConnections)
            {
                networkView.RPC("StartRace", RPCMode.AllBuffered);
            }

//#if UNITY_EDITOR

        }

//#endif

    }

    #endregion
}