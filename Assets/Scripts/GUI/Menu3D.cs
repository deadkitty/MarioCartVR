//using UnityEngine;
//using System.Collections;

//public class Menu3D : MonoBehaviour 
//{
//    public GameObject currentMenu;
//    public GameObject currentButton;

//    private GameObject mainMenu;
//    private GameObject gamemodeMenu;
//    private GameObject ingameMenu;
//    private GameObject popupWinMenu;
//    private GameObject popupLostMenu;

//    private Menu[] menus;
//    private Menu currentSubmenu;

//    private bool canToggle = false;
//    private bool canGoBack = false;
//    private bool canSwitchButton = true;

//    private bool startServer = false;
    
//    void Start () 
//    {

//        //mainMenu      = transform.FindChild("MainMenu").gameObject;
//        //gamemodeMenu  = transform.FindChild("GamemodeMenu").gameObject;
//        //ingameMenu    = transform.FindChild("IngameMenu").gameObject;
//        //popupWinMenu  = transform.FindChild("PopupWonGame").gameObject;
//        //popupLostMenu = transform.FindChild("PopupLostGame").gameObject;
//    }
	
//    void Update () 
//    {
//        if(currentMenu.activeSelf)
//        {
//            if (Input.GetAxis("NavigateMenu") < 0.5f && Input.GetAxis("NavigateMenu") > -0.5f)
//            {
//                canSwitchButton = true;
//            }
//            else if (Input.GetAxis("NavigateMenu") <= -1.0f && canSwitchButton)
//            {
//                canSwitchButton = false;
//                MenuUp();
//            }
//            else if (Input.GetAxis("NavigateMenu") >= 1.0f && canSwitchButton)
//            {
//                canSwitchButton = false;
//                MenuDown();
//            }

//            if (Input.GetButtonDown("Select"))
//            {
//                Select();
//            }

//            if (Input.GetButtonDown("GoBack") && canGoBack)
//            {
//                GoBack();
//            }
//        }

//        if(Input.GetButtonDown("ToggleMenu") && canToggle)
//        {
//            ToggleMenu();
//        }
//    }

//    public void Show(Menu.EType menu)
//    {
//        currentMenu.SetActive(false);
        
//        switch(menu)
//        {
//            case Menu.EType.main             : currentMenu = mainMenu     ; break;
//            case Menu.EType.gamemodeSelection: currentMenu = gamemodeMenu ; break;
//            case Menu.EType.ingame           : currentMenu = ingameMenu   ; break;
//            case Menu.EType.popupWin         : currentMenu = popupWinMenu ; break;
//            case Menu.EType.popupLost        : currentMenu = popupLostMenu; break;
//        }

//        currentMenu.SetActive(true);
//        currentSubmenu = currentMenu.GetComponent<Menu>();
//        currentSubmenu.ClearButtonColors();

//        currentButton = currentSubmenu.GetButton();
//    }

//    public void MenuUp()
//    {
//        currentButton = currentSubmenu.GetPrev();
//    }

//    public void MenuDown()
//    {
//        currentButton = currentSubmenu.GetNext();
//    }

//    public void ToggleMenu()
//    {
//        currentMenu.SetActive(!currentMenu.activeSelf);
//    }

//    public void GoBack()
//    {
//        Show(Menu.EType.main);
//    }

//    public void Select()
//    {
//        switch(currentSubmenu.CurrentButton)
//        {
//            case Menu.EButton.startgame: StartGame(); break;
//            case Menu.EButton.joingame: JoinGame(); break;
//            case Menu.EButton.exit: ExitGame(); break;

//            case Menu.EButton.racing: SelectRacing(); break;
//            case Menu.EButton.pursuit: SelectPursuit(); break;
//            case Menu.EButton.cancel: Cancel(); break;

//            case Menu.EButton.mainmenu: break;
//            case Menu.EButton.startnew: break;

//            case Menu.EButton.next: break;
//        }
//    }

//    private void StartGame()
//    {
//        startServer = true;
//        Show(Menu.EType.gamemodeSelection);
//    }

//    private void JoinGame()
//    {
//        startServer = false;
//        Show(Menu.EType.gamemodeSelection);
//    }

//    private void ExitGame()
//    {
//        Application.Quit();
//    }

//    private void SelectRacing()
//    {
//        if(startServer)
//        {
//            //NetworkManager.sInstance.StartServer();
//        }
//        else
//        {
//            //NetworkManager.sInstance.RefreshHostList();
//        }

//        Show(Menu.EType.ingame);

//        canToggle = true;
//        ToggleMenu();
//    }

//    private void SelectPursuit()
//    {
//        if (startServer)
//        {
//            //NetworkManager.sInstance.StartServer();
//        }
//        else
//        {
//            //NetworkManager.sInstance.RefreshHostList();
//            //NetworkManager.sInstance.JoinServer(NetworkManager.sInstance.HostList[0]);
//        }

//        Show(Menu.EType.ingame);

//        canToggle = true;
//        ToggleMenu();
//    }

//    private void Cancel()
//    {
//        Show(Menu.EType.main);
//    }
//}
