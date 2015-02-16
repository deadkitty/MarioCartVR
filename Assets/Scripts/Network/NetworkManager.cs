using UnityEngine;
using System;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    #region Fields

    private static NetworkManager sInstance;

    private const String typeName = "SuperVirtualCartGame";

    public GameObject cartPrefab;
    public GameObject heliPrefab;

    private GameObject playerCart;

    private static int playerIndex = -1;    
    private static int playersWhoWantToReset = 0;

    private static bool isHelicopter = false;

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
        Debug.Log("Start Server: " + App.GameName);

        Network.InitializeServer(App.MaxPlayers - 1, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, App.GameName);
    }

    public static void StopServer()
    {
        Network.Destroy(sInstance.playerCart);

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
        MasterServer.UnregisterHost();

        for (int i = 0; i < App.maxPlayers; ++i )
        {
            Destroy(App.GetPlayer(i));
        }
    }

    public static void Reset()
    {
        if (Network.isServer)
        {
            sInstance.PlayerWantsToReset(playerIndex);
        }
        else
        {
            sInstance.networkView.RPC("PlayerWantsToReset", RPCMode.Server, playerIndex);
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

        if(isHelicopter)
        {
            sInstance.playerCart = Network.Instantiate(sInstance.heliPrefab, spawn.position, spawn.rotation, 0) as GameObject;
        }
        else
        {
            sInstance.playerCart = Network.Instantiate(sInstance.cartPrefab, spawn.position, spawn.rotation, 0) as GameObject;
        }
    }

    #endregion

    #region RPC Functions

    [RPC]
    void SetPlayerIndex(int index)
    {
        Debug.Log("SetPlayerIndex " + index);

        playerIndex = index;

        if(App.GameMode == App.EGameMode.pursuit && playerIndex == 1)
        {
            isHelicopter = true;
        }

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
    void PlayerWantsToReset(int playerIndex)
    {
        Debug.Log("Player " + playerIndex + " wants to reset");

        ++playersWhoWantToReset;

        if (playersWhoWantToReset == Network.maxConnections + 1)
        {
            sInstance.networkView.RPC("ResetPlayer", RPCMode.AllBuffered);
        }
    }

    [RPC]
    void ResetPlayer()
    {
        Debug.Log("ResetGame");

        Network.Destroy(playerCart);

        playersWhoWantToReset = 0;

        LapCounter.Reset();
        CartTimer.Reset();

        StartRace();
    }

    [RPC]
    void Ready()
    {
        Debug.Log("Player is Ready");
        Debug.Log("Connections: " + Network.connections.Length + " MaxConnections: " + Network.maxConnections);

#if UNITY_EDITOR

        if (startOffline)
        {
            StartRace();
        }

        if (!startOffline)
        {

#endif

            if (Network.connections.Length == Network.maxConnections)
            {
                networkView.RPC("StartRace", RPCMode.AllBuffered);
            }

#if UNITY_EDITOR

        }

#endif

    }

    #endregion
}