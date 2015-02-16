using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
    #region Fields

    private static MenuController sInstance;

    public GameObject menuObject;

    private static Menu[] menus;
    private static Menu currentMenu;

    private static bool canToggle = false;
    private static bool canGoBack = false;
    private static bool canSwitchButton = true;

    private static bool startServer = false;
    
    #endregion

    #region Events

    void Start()
    {
        Debug.Log("MenuController.Start");
        sInstance = this;
    }

    void Update()
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
    
    #endregion

    #region Class Functions

    public static void Initialize()
    {
        Debug.Log("MenuController.Initialize");
        menus = new Menu[sInstance.menuObject.transform.childCount];

        for (int i = 0; i < menus.Length; ++i)
        {
            menus[i] = sInstance.menuObject.transform.GetChild(i).GetComponent<Menu>();
        }
        
        currentMenu = menus[0];
        ShowMenu(App.CurrentMenu);
    }

    public static void ShowMenu(Menu.EType type)
    {
        currentMenu.ClearButtonColors();
        currentMenu.gameObject.SetActive(false);

        switch (type)
        {
            case Menu.EType.main:

                currentMenu = menus[0];
                canToggle = false;
                canGoBack = false;

                break;

            case Menu.EType.gamemodeSelection:

                currentMenu = menus[1];
                canToggle = false;
                canGoBack = true;

                break;

            case Menu.EType.levelSelection:

                currentMenu = menus[2];
                canToggle = false;
                canGoBack = true;

                break;

            case Menu.EType.ingame:

                currentMenu = menus[3];
                canToggle = true;
                canGoBack = false;

                break;

            case Menu.EType.popupWin:

                currentMenu = menus[4];
                canToggle = false;
                canGoBack = false;

                break;

            case Menu.EType.popupLost:

                currentMenu = menus[5];
                canToggle = false;
                canGoBack = false;

                break;

            case Menu.EType.none:

                return;
        }

        currentMenu.gameObject.SetActive(true);
        currentMenu.GetButton();

        App.CurrentMenu = type;
    }
   
    #endregion

    #region Member Functions

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
        Debug.Log("GoBack: " + currentMenu.type.ToString());

        switch(currentMenu.type)
        {
            case Menu.EType.gamemodeSelection: ShowMenu(Menu.EType.main); break;
            case Menu.EType.levelSelection   : ShowMenu(Menu.EType.gamemodeSelection); break;
        }
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

            case Menu.EButton.level1: LoadLevel(App.ELevel.level1); break;
            case Menu.EButton.level2: LoadLevel(App.ELevel.level2); break;

            case Menu.EButton.mainmenu: MainMenu(); break;
            case Menu.EButton.startnew: StartNew(); break;

            case Menu.EButton.next: Next(); break;
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
        App.GameMode = App.EGameMode.racing;
        App.GameName = App.EGameMode.racing.ToString();
        ShowMenu(Menu.EType.levelSelection);
    }

    private void SelectPursuit()
    {
        App.GameMode = App.EGameMode.pursuit;
        App.GameName = App.EGameMode.pursuit.ToString();
        ShowMenu(Menu.EType.levelSelection);
    }

    private void Cancel()
    {
        GoBack();
    }

    private void LoadLevel(App.ELevel level)
    {
        App.Level = level;
        App.GameName += level.ToString();
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
        ToggleMenu();
    }

    private void MainMenu()
    {
        if (Network.isServer)
        {
            NetworkManager.StopServer();
        }
        else
        {
            NetworkManager.LeaveServer();
        }

        App.LoadLevel(App.ELevel.none);

        ShowMenu(Menu.EType.main);
    }

    private void StartNew()
    {
        if (LapCounter.raceFinished)
        {
            NetworkManager.Reset();

            ToggleMenu();
        }
    }

    private void Next()
    {
        ShowMenu(Menu.EType.ingame);
    }
    
    #endregion
}
