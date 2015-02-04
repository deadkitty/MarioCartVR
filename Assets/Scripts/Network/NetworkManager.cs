using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager sInstance;

    private String typeName = "SuperVirtualCartGame";
    private String gameName = "SuperVirtualCart";
    
    public GameObject playerPrefab;
    public GameObject[] spawns;
    
    private HostData[] hostList;
    private int playersConnected = 0;
    public int playerID = -1;

    public bool allPlayersConnected = false;


    public static HostData[] HostList
    {
        get { return sInstance.hostList; }
    }
        
    public static int PlayersConnected
    {
        get { return sInstance.playersConnected; }
        set
        {
            sInstance.playersConnected = value;
            CheckAllPlayersConnected();
        }
    }

    public static int PlayerID
    {
        get { return sInstance.playerID; }
    }

    public static String GameName
    {
        get { return sInstance.gameName; }
        set { sInstance.gameName = value; }
    } 
    
    void Start()
    {
        sInstance = this;
    }

    
    public static void StartServer()
    {
        Network.InitializeServer(Players.playerCount, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(sInstance.typeName, sInstance.gameName);
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
        Network.Connect(host);
    }

    public static void LeaveServer()
    {

    }

    private static void CheckAllPlayersConnected()
    {
        if (PlayersConnected == Players.playerCount)
        {
            sInstance.networkView.RPC("StartRace", RPCMode.AllBuffered);
        }
    }

    private static void SpawnPlayer()
    {
        Transform spawn = sInstance.spawns[PlayerID].transform;
        Network.Instantiate(sInstance.playerPrefab, spawn.position, spawn.rotation, 0);

        //if (Network.isServer)
        //{
        //    Debug.Log("Server: Spawn Player");

        //    Network.Instantiate(playerPrefab, spawn1.transform.position, spawn1.transform.rotation, 0);
        //}
        //else
        //{
        //    Debug.Log("Client: Spawn Player");

        //    Network.Instantiate(playerPrefab, spawn2.transform.position, spawn2.transform.rotation, 0);
        //}
    }


    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            Debug.Log("Hostlist Received");
            hostList = MasterServer.PollHostList();
            
            foreach(HostData data in hostList)
            {
                if(data.gameName == gameName && data.connectedPlayers < data.playerLimit)
                {
                    JoinServer(data);
                }
            }
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        
        SetPlayerID(Network.player.ipAddress, playersConnected);

        ++PlayersConnected;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playersConnected + " connected from " + player.ipAddress + ":" + player.port);

        networkView.RPC("SetPlayerID", RPCMode.OthersBuffered, player.ipAddress, playersConnected);

        ++PlayersConnected;
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);

        --PlayersConnected;
    }

    [RPC]
    void SetPlayerID(string ipAddress, int id)
    {
        Debug.Log("SetPlayerID");

        if(ipAddress == Network.player.ipAddress)
        {
            playerID = id;
        }
    }

    [RPC]
    void StartRace()
    {
        SpawnPlayer();
        CartTimer.StartRaceTimer();
    }
}

//using UnityEngine;
//using System.Collections;
//using System;

//public class NetworkManager : MonoBehaviour
//{
//    private const String typeName = "SuperVirtualCartGame";
//    private const String gameName = "SuperVirtualCart";
	
//    private HostData[] hostList;
	
//    public HostData[] HostList
//    {
//        get { return hostList; }
//    }
	
//    public GameObject playerPrefab;
//    public GameObject spawn1;
//    public GameObject spawn2;
	
//    private int playersConnected = 0;
	
//    public int PlayersConnected
//    {
//        get { return playersConnected; }
//        set
//        {
//            playersConnected = value;
//            CheckAllPlayersConnected();
//        }
//    }
	
//    public bool allPlayersConnected = false;
	
//    public static NetworkManager sInstance;
	
//    void Start()
//    {
//        sInstance = this;
		
//        RefreshHostList();
//    }
	
//    public void StartServer()
//    {
//        Network.InitializeServer(Players.playerCount, 25000, !Network.HavePublicAddress());
//        MasterServer.RegisterHost(typeName, gameName);
//    }
	
//    public void StopServer()
//    {
//        Network.Disconnect();
//        MasterServer.UnregisterHost();
//    }
	
//    public void RefreshHostList()
//    {
//        MasterServer.RequestHostList(typeName);
//    }
	
//    public void JoinServer(HostData host)
//    {
//        Network.Connect(host);
//    }
	
//    void OnMasterServerEvent(MasterServerEvent msEvent)
//    {
//        if (msEvent == MasterServerEvent.HostListReceived)
//        {
//            Debug.Log("Hostlist Received");
//            hostList = MasterServer.PollHostList();
			
//            if(hostList.Length == 0)
//            {
//                StartServer();
//            }
//            else
//            {
//                JoinServer(hostList[0]);
//            }
//        }
//    }
	
//    void OnServerInitialized()
//    {
//        Debug.Log("Server Initializied");
		
//        ++PlayersConnected;
//    }
	
//    void CheckAllPlayersConnected()
//    {
//        if (playersConnected == Players.playerCount)
//        {
//            networkView.RPC("StartRace", RPCMode.AllBuffered);
//        }
//    }
	
//    void OnPlayerConnected(NetworkPlayer player)
//    {
//        Debug.Log("Player " + playersConnected + " connected from " + player.ipAddress + ":" + player.port);
		
//        ++PlayersConnected;
//    }
	
//    [RPC]
//    void StartRace()
//    {
//        SpawnPlayer();
		
//        CartTimer.StartRaceTimer();
		
//        MainMenu.HideMenu();
//    }
	
//    void SpawnPlayer()
//    {
//        if (Network.isServer)
//        {
//            Debug.Log("Server: Spawn Player");
			
//            Network.Instantiate(playerPrefab, spawn1.transform.position, spawn1.transform.rotation, 0);
//        }
//        else
//        {
//            Debug.Log("Client: Spawn Player");
			
//            Network.Instantiate(playerPrefab, spawn2.transform.position, spawn2.transform.rotation, 0);
//        }
//    }
//}
