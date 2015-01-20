using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour 
{
    private const String typeName = "SuperVirtualCartGame";
    private const String gameName = "SuperVirtualCart";

    private HostData[] hostList;

    public GameObject playerPrefab;
    public GameObject spawn1;
    public GameObject spawn2;

    public GameObject[] players;

    public const int playersCount = 2;
    public int playersConnected = 0;

    public bool allPlayersConnected = false;

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
	
	void Update () 
    {
	    
	}

    private void StartServer()
    {
        Network.InitializeServer(playersCount, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    private void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    private void JoinServer(HostData host)
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

    void OnGUI()
    {
        if(!Network.isServer && !Network.isClient)
        {
            if (GUI.Button(new Rect(25, 80, 150, 30), "Start Server"))
            {
                StartServer();
            }

            if (GUI.Button(new Rect(25, 130, 150, 30), "Refresh Hosts"))
            {
                RefreshHostList();
            }

            if(hostList != null)
            {
                for (int i = 0; i < hostList.Length; ++i)
                {
                    if (GUI.Button(new Rect(25, 180 + i * 50, 150, 30), "Join Server"))
                    {
                        JoinServer(hostList[i]);
                    }
                }
            }
        }
    }
}
