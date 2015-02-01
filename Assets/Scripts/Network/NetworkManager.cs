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

    private int playersConnected = 0;

    public int PlayersConnected
    {
        get { return playersConnected; }
        set 
        { 
            playersConnected = value;
            CheckAllPlayersConnected();
        }
    }

    public bool allPlayersConnected = false;

    public static NetworkManager sInstance;

	void Start () 
    {
        sInstance = this;
	}
	
    public void StartServer()
    {
        Network.InitializeServer(Players.playerCount, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    public void StopServer()
    {
        Network.Disconnect();
        MasterServer.UnregisterHost();
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

        ++PlayersConnected;
    }

    void CheckAllPlayersConnected()
    {
        if (playersConnected == Players.playerCount)
        {
            networkView.RPC("StartRace", RPCMode.AllBuffered);
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playersConnected + " connected from " + player.ipAddress + ":" + player.port);

        ++PlayersConnected;
    }

    [RPC]
    void StartRace()
    {
        SpawnPlayer();

        CartTimer.StartRaceTimer();

        MainMenu.HideMenu();
    }

    void SpawnPlayer()
    {     
        if(Network.isServer)
        {
            Debug.Log("Server: Spawn Player");

            Network.Instantiate(playerPrefab, spawn1.transform.position, spawn1.transform.rotation, 0);
        }
        else
        {
            Debug.Log("Client: Spawn Player");

            Network.Instantiate(playerPrefab, spawn2.transform.position, spawn2.transform.rotation, 0);
        }
    }
}
