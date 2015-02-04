using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour 
{
	GameObject starttext;

    private static bool startedNew = true;

	// Use this for initialization
	void Start ()
    {
        starttext = GameObject.Find("starttext");

        if(startedNew)
        {
            InvokeRepeating("FlashLabel", 0, 1);
            startedNew = false;
        }
        else
        {
            starttext.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("ResetCamera") && starttext.activeSelf)
		{
            CancelInvoke("FlashLabel");
            starttext.SetActive(false);
            MenuController.ShowMenu(Menu.EType.main);
		}
	}

	void FlashLabel()
	{
		if(starttext.activeSelf)
		{
			starttext.SetActive(false);
		}
		else
		{
			starttext.SetActive(true);
		}
	}
}
