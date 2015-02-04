using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour 
{
    private static Startup sInstance;

    public static Menu.EType currentMenu = Menu.EType.main;

	void Start () 
    {
        sInstance = this;
	}
}
