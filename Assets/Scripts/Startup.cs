using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour 
{
    private static Startup sInstance;

    private static Menu.EType currentMenu = Menu.EType.none;
    private static string gameName;

    private static bool levelLoaded = false;

    public static Menu.EType CurrentMenu
    {
        get { return currentMenu; }
        set { currentMenu = value; }
    }

    public static string GameName
    {
        get { return gameName; }
        set { gameName = value; }
    }

	void Start () 
    {
        Debug.Log("Startup.Start");

        sInstance = this;

        if(levelLoaded)
        {
            NetworkManager.PlayerReady();
        }
	}

    public static void LoadLevel()
    {
        if (gameName == "Racing")
        {
            Application.LoadLevel("RacingLevel");
        }
        else
        {
            Application.LoadLevel("PursuitLevel");
        }
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("Startup.OnLevelWasLoaded");

        if (Application.loadedLevelName != "MainMenu")
        {
            levelLoaded = true;
        }
    }
}
