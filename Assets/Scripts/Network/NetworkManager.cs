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
    
    private static int currentRound = 0;
    private static float[] raceTimes = new float[App.MaxPlayers];

#if UNITY_EDITOR

    public bool startOffline = false;

#endif

    #endregion

    #region Properties

    public static int PlayerIndex
    {
        get { return playerIndex; }
    }

    public static int CurrentRound
    {
        get { return NetworkManager.currentRound; }
        set
        {
            Debug.Log("Current Round Changed, Old Value: " + currentRound + " New Value: " + value);
            currentRound = value;
        }
    }

    public static float[] RaceTimes
    {
        get { return NetworkManager.raceTimes; }
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

    public static void RoundFinished()
    {
        if(!isHelicopter)
        {
            if (Network.isServer)
            {
                sInstance.RoundFinishedRPC(CartTimer.CurrentTime);
            }
            else
            {
                sInstance.networkView.RPC("RoundFinishedRPC", RPCMode.Server, CartTimer.CurrentTime);
            }
        }

        if (CurrentRound == 0)
        {
            MenuController.ShowMenu(Menu.EType.popupNext);
        }
    }

    private static void SpawnPlayer()
    {
        if(App.GameMode == App.EGameMode.racing)
        {
            Transform spawn = App.Spawns[playerIndex].transform;
            sInstance.playerCart = Network.Instantiate(sInstance.cartPrefab, spawn.position, spawn.rotation, 0) as GameObject;
        }
        else
        {
            if (isHelicopter)
            {
                Transform spawn = App.Spawns[1].transform;
                sInstance.playerCart = Network.Instantiate(sInstance.heliPrefab, spawn.position, spawn.rotation, 0) as GameObject;
            }
            else
            {
                Transform spawn = App.Spawns[0].transform;
                sInstance.playerCart = Network.Instantiate(sInstance.cartPrefab, spawn.position, spawn.rotation, 0) as GameObject;
            }
        }
    }

    #endregion

    #region RPC Functions

    //Server
    [RPC]
    void SetPlayerIndex(int index)
    {
        Debug.Log("SetPlayerIndex " + index);

        playerIndex = index;
        
        PlayerReady();
    }

    //All
    [RPC]
    void StartRace()
    {
        Debug.Log("StartRace");

        if(App.GameMode == App.EGameMode.pursuit)
        {
            if(Network.isServer)
            {
                if(currentRound == 0)
                {
                    isHelicopter = false;
                }
                else
                {
                    isHelicopter = true;
                }
            }
            else
            {
                if (currentRound == 0)
                {
                    isHelicopter = true;
                }
                else
                {
                    isHelicopter = false;
                }
            }
        }

        SpawnPlayer();
        CartTimer.StartRaceTimer();
    }

    //Server
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

    //All
    [RPC]
    void ResetPlayer()
    {
        Debug.Log("ResetGame");

        Network.Destroy(playerCart);

        playersWhoWantToReset = 0;

        if(!LapCounter.raceFinished)
        {
            ++CurrentRound;
        }

        LapCounter.Reset();
        CartTimer.Reset();
        
        StartRace();
    }

    //Server
    [RPC]
    void RoundFinishedRPC(float time)
    {
        Debug.Log("Round " + CurrentRound + " Finished, Time: " + time);

        raceTimes[CurrentRound] = time;

        if(CurrentRound == 1)
        {
            //server cart was faster than client cart
            if (raceTimes[0] < raceTimes[1])
            {
                PursuitFinished(true);
                networkView.RPC("PursuitFinished", Network.connections[0], false);
            }
            else
            {
                PursuitFinished(false);
                networkView.RPC("PursuitFinished", Network.connections[0], true);
            }
        }
    }
    
    //All
    [RPC]
    void PursuitFinished(bool won)
    {
        Debug.Log("Pursuit Finished. Won Race:" + won);

        if(won)
        {
            MenuController.ShowMenu(Menu.EType.popupWin);
        }
        else
        {
            MenuController.ShowMenu(Menu.EType.popupLost);
        }

        raceTimes[0] = 0.0f;
        raceTimes[1] = 0.0f;

        LapCounter.raceFinished = true;
    }

    //Server
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