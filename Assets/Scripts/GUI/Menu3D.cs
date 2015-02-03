using UnityEngine;
using System.Collections;

public class Menu3D : MonoBehaviour 
{
    public enum EMenu
    {
        main,
        gamemodeSelection,
        ingame,
        popupWin,
        popupLost,
        none,
    }

    private static Menu3D sInstance;

    public EMenu currentMenuType;

    public GameObject currentMenu;
    public GameObject currentButton;

    private GameObject mainMenu;
    private GameObject gamemodeMenu;
    private GameObject ingameMenu;
    private GameObject popupWinMenu;
    private GameObject popupLostMenu;

    private Submenu currentSubmenu;

    private bool canToggle = false;
    private bool canGoBack = false;
    private bool canSwitchButton = true;

    private bool navigationState = true;
    private bool lastNavigationState = true;

	void Start () 
    {
        sInstance = this;
        mainMenu      = transform.FindChild("MainMenu").gameObject;
        gamemodeMenu  = transform.FindChild("GamemodeMenu").gameObject;
        ingameMenu    = transform.FindChild("IngameMenu").gameObject;
        popupWinMenu  = transform.FindChild("PopupWonGame").gameObject;
        popupLostMenu = transform.FindChild("PopupLostGame").gameObject;

        Show(EMenu.main);
        currentButton = currentSubmenu.GetButton();
        currentButton.renderer.material.color = Color.green;
	}
	
	void Update () 
    {
        if(currentMenu.activeSelf)
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

        if(Input.GetButtonDown("ToggleMenu") && canToggle)
        {
            ToggleMenu();
        }
	}

    public void Show(EMenu menu)
    {
        if(sInstance.currentMenu != null)
        {
            sInstance.currentMenu.SetActive(false);
        }

        switch(menu)
        {
            case EMenu.main             : sInstance.currentMenu = sInstance.mainMenu     ; break;
            case EMenu.gamemodeSelection: sInstance.currentMenu = sInstance.gamemodeMenu ; break;
            case EMenu.ingame           : sInstance.currentMenu = sInstance.ingameMenu   ; break;
            case EMenu.popupWin         : sInstance.currentMenu = sInstance.popupWinMenu ; break;
            case EMenu.popupLost        : sInstance.currentMenu = sInstance.popupLostMenu; break;
            case EMenu.none             : sInstance.currentMenu = null                   ; break;
        }

        sInstance.currentMenuType = menu;

        if(sInstance.currentMenu != null)
        {
            sInstance.currentMenu.SetActive(true);
            sInstance.currentSubmenu = sInstance.currentMenu.GetComponent<Submenu>();
        }
    }

    public void Show(GameObject menu)
    {
        if (sInstance.currentMenu != null)
        {
            sInstance.currentMenu.SetActive(false);
        }

        sInstance.currentMenu = menu;
        sInstance.currentMenu.SetActive(true);
    }

    public void MenuUp()
    {
        currentButton.renderer.material.color = Color.white;
        currentButton = currentSubmenu.GetPrev();
        currentButton.renderer.material.color = Color.green;
    }

    public void MenuDown()
    {
        currentButton.renderer.material.color = Color.white;
        currentButton = currentSubmenu.GetNext();
        currentButton.renderer.material.color = Color.green;
    }

    public void ToggleMenu()
    {
        sInstance.currentMenu.SetActive(!sInstance.currentMenu.activeSelf);
    }

    public void Select()
    {

    }

    public void GoBack()
    {

    }
}
