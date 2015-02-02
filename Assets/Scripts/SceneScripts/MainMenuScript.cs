using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	GameObject starttext;

	// Use this for initialization
	void Start () 
	{
		starttext = GameObject.Find ("starttext");
		InvokeRepeating("FlashLabel", 0, 1);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Return))
		{
			Application.LoadLevel(1);
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
