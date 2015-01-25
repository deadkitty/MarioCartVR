using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour 
{
    private const String typeName = "SuperVirtualCartGame";
    private const String gameName = "SuperVirtualCart";

    private HostData[] hostList;

    public HostData[] HostList
    {
        get { return hostList; }
    }

    public GameObject playerPrefab;
    public GameObject spawn1;
    public GameObject spawn2;

    public GameObject[] players;

    public const int playersCount = 2;
    public int playersConnected = 0;

    public bool allPlayersConnected = false;

    //No real singelton because we just need access to a networkmanager gameobject, but it doesnt matter if there is one or more instances of the object
    public static NetworkManager sInstance;

	void Start () 
    {
        sInstance = this;

        RefreshHostList();

        if (hostList != null)
        {
            for (int i = 0; i < hostList.Length; ++i)
            {
                if (GUI.Button(new Rect(25, 135 + i * 55, 150, 30), "Join Server"))
                {
                    JoinServer(hostList[i]);
                }
            }
        }

        players = new GameObject[playersCount];
	}
	
    public void StartServer()
    {
        Network.InitializeServer(playersCount, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    public void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    public void JoinServer(HostData host)
    {
        Network.Connect(host);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if(msEvent == MasterServerEvent.HostListReceived)
        {
            Debug.Log("Hostlist Received");
            hostList = MasterServer.PollHostList();
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");

        ++playersConnected;
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playersConnected + " connected from " + player.ipAddress + ":" + player.port);

        ++playersConnected;

        if (playersConnected == playersCount)
        {
            networkView.RPC("StartRace", RPCMode.AllBuffered);
        }
    }

    [RPC]
    void StartRace()
    {
        SpawnPlayer();

        CartTimer.StartRace();

        MainMenu.HideMenu();
    }

    void SpawnPlayer()
    {
        Debug.Log("Spawn Player");

        if(Network.isServer)
        {
            Network.Instantiate(playerPrefab, spawn1.transform.position, spawn1.transform.rotation, 0);
        }
        else
        {
            Network.Instantiate(playerPrefab, spawn2.transform.position, spawn1.transform.rotation, 0);
        }
    }
}
