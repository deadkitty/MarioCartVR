using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    public int buttonWidth = 200;
    public int buttonHeight = 35;

    private NetworkManager networkManager;
    
    public enum EPage
    {
        startMenu,
        selectHost,
        waitForPlayers,
        inGameMenu,
        none,
    }

    private EPage currentPage = EPage.startMenu;
    private EPage lastPage = EPage.none;

    private static MainMenu sInstance;

	void Start () 
    {
        sInstance = this;
        
        networkManager = NetworkManager.sInstance;
	}
	
    void OnGUI()
    {        
        switch(currentPage)
        {
            case EPage.startMenu     : ShowStartMenu()         ; break;
            case EPage.selectHost    : ShowSelectHostMenu()    ; break;
            case EPage.waitForPlayers: ShowWaitForPlayersMenu(); break;
        }
    }

    void ShowStartMenu()
    {
        int buttonPositionX = Screen.width / 2 - buttonWidth / 2;
        int buttonPositionY = 100;

        if (GUI.Button(new Rect(buttonPositionX, buttonPositionY, buttonWidth, buttonHeight), "Spiel starten"))
        {
            networkManager.StartServer();
            currentPage = EPage.waitForPlayers;
        }

        buttonPositionY += buttonHeight + 30;
        if (GUI.Button(new Rect(buttonPositionX, buttonPositionY, buttonWidth, buttonHeight), "Spiel beitreten"))
        {
            networkManager.RefreshHostList();
            currentPage = EPage.selectHost;
        }

        buttonPositionY += buttonHeight + 30;
        if (GUI.Button(new Rect(buttonPositionX, buttonPositionY, buttonWidth, buttonHeight), "Beenden"))
        {
            Application.Quit();
        }
    }

    void ShowSelectHostMenu()
    {
        int buttonPositionX = Screen.width / 2 - buttonWidth / 2;
        int buttonPositionY = 100;

        if (networkManager.HostList != null)
        {
            for (int i = 0; i < networkManager.HostList.Length; ++i)
            {
                buttonPositionY += buttonHeight + 20;
                if (GUI.Button(new Rect(buttonPositionX, buttonPositionY, buttonWidth, buttonHeight), "Spiel beitreten"))
                {
                    currentPage = EPage.none;
                    networkManager.JoinServer(networkManager.HostList[i]);
                }
            }
        }

        if (GUI.Button(new Rect(buttonPositionX + buttonWidth + 20, buttonPositionY, buttonWidth, buttonHeight), "abbrechen"))
        {
            currentPage = EPage.startMenu;
        }
    }

    void ShowWaitForPlayersMenu()
    {
        if (GUI.Button(new Rect(25, 180, 150, 30), "abbrechen"))
        {
            currentPage = EPage.startMenu;
            networkManager.StopServer();
        }
    }

    public static void HideMenu()
    {
        sInstance.lastPage = sInstance.currentPage;
        sInstance.currentPage = EPage.none;
    }

    public static void ShowMenu()
    {
        sInstance.currentPage = sInstance.lastPage;
    }
}
