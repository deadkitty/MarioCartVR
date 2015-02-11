using UnityEngine;
using System.Collections;
using System;

public class App : MonoBehaviour 
{
    #region EGameMode

    public enum EGameMode
    {
        racing,
        pursuit,
        none
    }

    #endregion

    #region ELevel
    
    public enum ELevel
    {
        level1,
        level2,
        none,
    }

    #endregion

    #region Fields

    private static App sInstance;

    public static int maxPlayers = 2;

    private static EGameMode gameMode;
    private static String    gameName;
    
    private static ELevel     currentLevel;
    private static GameObject currentLevelObject;
    private static Menu.EType currentMenu;
        
    private static Transform cameraStartPos;

    public GameObject[] levels;
    public GameObject[] spawns;
    public GameObject   camera;

    #endregion

    #region Properties

    public static int MaxPlayers
    {
        get { return App.maxPlayers; }
    }

    public static EGameMode GameMode
    {
        get { return App.gameMode; }
        set 
        { 
            App.gameMode = value;
            App.gameName = value.ToString();
        }
    }

    public static ELevel Level
    {
        get { return App.currentLevel; }
        set { App.currentLevel = value; }
    }

    public static Menu.EType CurrentMenu
    {
        get { return App.currentMenu; }
        set { App.currentMenu = value; }
    }

    public static String GameName
    {
        get { return App.gameName; }
    }

    public static Transform CameraStartPos
    {
        get { return App.cameraStartPos; }
        set { App.cameraStartPos = value; }
    }
    
    public static GameObject[] Spawns
    {
        get { return sInstance.spawns; }
        set { sInstance.spawns = value; }
    }

    #endregion

    #region Events

    void Start()
    {
        Debug.Log("App.Start");
        sInstance = this;

        gameMode     = EGameMode.none;
        currentMenu  = Menu.EType.none;
        currentLevel = ELevel.none;

        spawns = new GameObject[MaxPlayers];

        foreach (GameObject level in levels)
        {
            level.SetActive(false);
        }

        currentLevelObject = levels[0];
        LoadLevel(currentLevel);

        MenuController.Initialize();
    }
    
    #endregion

    #region Class Functions

    public static void LoadLevel(ELevel level)
    {
        //Switch correct Level Geometry On/Off
        switch(level)
        {
            case ELevel.level1: LoadLevel1(); break;
            case ELevel.level2: LoadLevel2(); break;
            case ELevel.none  : LoadNone  (); break;
        }

        currentLevel = level;

        //Set Camera Starting Position
        cameraStartPos = currentLevelObject.transform.FindChild("CameraStartPos");

        sInstance.camera.transform.parent = null;
        sInstance.camera.transform.position = cameraStartPos.position;
        sInstance.camera.transform.rotation = cameraStartPos.rotation;
    }

    private static void LoadLevel1()
    {
        currentLevelObject.SetActive(false);
        currentLevelObject = sInstance.levels[0];
        currentLevelObject.SetActive(true);
        FindSpawns();
    }

    private static void LoadLevel2()
    {
        currentLevelObject.SetActive(false);
        currentLevelObject = sInstance.levels[1];
        currentLevelObject.SetActive(true);
        FindSpawns();
    }

    private static void LoadNone()
    {
        currentLevelObject.SetActive(false);
        currentLevelObject = sInstance.levels[2];
        currentLevelObject.SetActive(true);
    }

    private static void FindSpawns()
    {
        Transform spawn = currentLevelObject.transform.FindChild("Spawns");

        for (int i = 0; i < MaxPlayers; ++i)
        {
            sInstance.spawns[i] = spawn.GetChild(i).gameObject;
        }
    }

    #endregion
}
