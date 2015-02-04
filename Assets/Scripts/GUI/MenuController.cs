using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
    private static MenuController sInstance;

    public GameObject menuObject;

    private Menu[] menus;
    private Menu currentMenu;

    private bool canToggle = false;
    private bool canGoBack = false;
    private bool canSwitchButton = true;

    private bool startServer = false;

	void Start () 
    {
        sInstance = this;

        menus = new Menu[menuObject.transform.childCount];
        for (int i = 0; i < menus.Length; ++i)
        {
            menus[i] = menuObject.transform.GetChild(i).GetComponent<Menu>();
        }

        currentMenu = menus[0];
        ShowMenu(Startup.currentMenu);
	}

	void Update ()
    {
        if (currentMenu.gameObject.activeSelf)
        {
            if (Input.GetAxis("NavigateMenu") < 0.5f && Input.GetAxis("NavigateMenu") > -0.5f)
            {
                canSwitchButton = true;
            }
            else if (Input.GetAxis("NavigateMenu") <= -1.0f && canSwitchButton)
            {
                canSwitchButton = false;
                MenuUp();
            }
            else if (Input.GetAxis("NavigateMenu") >= 1.0f && canSwitchButton)
            {
                canSwitchButton = false;
                MenuDown();
            }

            if (Input.GetButtonDown("Select"))
            {
                Select();
            }

            if (Input.GetButtonDown("GoBack") && canGoBack)
            {
                GoBack();
            }
        }

        if (Input.GetButtonDown("ToggleMenu") && canToggle)
        {
            ToggleMenu();
        }
    }

    public static void ShowMenu(Menu.EType type)
    {
        sInstance.currentMenu.ClearButtonColors();
        sInstance.currentMenu.gameObject.SetActive(false);

        switch(type)
        {
            case Menu.EType.main: 
                
                sInstance.currentMenu = sInstance.menus[0];
                sInstance.canToggle = false;
                sInstance.canGoBack = false;

                break;

            case Menu.EType.gamemodeSelection:

                sInstance.currentMenu = sInstance.menus[1];
                sInstance.canToggle = false;
                sInstance.canGoBack = true;

                break;

            case Menu.EType.ingame:
                
                sInstance.currentMenu = sInstance.menus[2];
                sInstance.canToggle = true;
                sInstance.canGoBack = false;

                break;

            case Menu.EType.popupWin:
                
                sInstance.currentMenu = sInstance.menus[3];
                sInstance.canToggle = false;
                sInstance.canGoBack = false;

                break;

            case Menu.EType.popupLost:

                sInstance.currentMenu = sInstance.menus[4];
                sInstance.canToggle = false;
                sInstance.canGoBack = false;

                break;
        }

        sInstance.currentMenu.gameObject.SetActive(true);
        sInstance.currentMenu.GetButton();

        Startup.currentMenu = type;
    }



    public void MenuUp()
    {
        currentMenu.GetPrev();
    }

    public void MenuDown()
    {
        currentMenu.GetNext();
    }

    public void ToggleMenu()
    {
        currentMenu.gameObject.SetActive(!currentMenu.gameObject.activeSelf);
    }

    public void GoBack()
    {
        ShowMenu(Menu.EType.main);
    }

    public void Select()
    {
        switch (currentMenu.CurrentButton)
        {
            case Menu.EButton.startgame: StartGame(); break;
            case Menu.EButton.joingame: JoinGame(); break;
            case Menu.EButton.exit: ExitGame(); break;

            case Menu.EButton.racing: SelectRacing(); break;
            case Menu.EButton.pursuit: SelectPursuit(); break;
            case Menu.EButton.cancel: Cancel(); break;

            case Menu.EButton.mainmenu: MainMenu(); break;
            case Menu.EButton.startnew: break;

            case Menu.EButton.next: break;
        }
    }

    private void StartGame()
    {
        startServer = true;
        ShowMenu(Menu.EType.gamemodeSelection);
    }

    private void JoinGame()
    {
        startServer = false;
        ShowMenu(Menu.EType.gamemodeSelection);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void SelectRacing()
    {
        NetworkManager.GameName = "Racing";
        InstanciateGame();
    }

    private void SelectPursuit()
    {
        NetworkManager.GameName = "Pursuit";
        InstanciateGame();
    }

    private void InstanciateGame()
    {
        if (startServer)
        {
            NetworkManager.StartServer();
        }
        else
        {
            NetworkManager.RefreshHostList();
        }

        ShowMenu(Menu.EType.ingame);

        canToggle = true;
        ToggleMenu();
    }

    private void Cancel()
    {
        ShowMenu(Menu.EType.main);
    }

    private void MainMenu()
    {
        if(Network.isServer)
        {
            NetworkManager.StopServer();
        }
        else
        {
            NetworkManager.LeaveServer();
        }

        ShowMenu(Menu.EType.main);
    }
}
