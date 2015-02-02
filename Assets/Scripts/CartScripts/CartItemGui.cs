using UnityEngine;
using System.Collections;

public class CartItemGui : MonoBehaviour 
{

	public int collecteditem = 1;
	private GameObject guimushroom;
	private GameObject guishell;

	// Use this for initialization
	void Start () 
	{
		guimushroom = GameObject.Find ("guimushroom");
		guishell = GameObject.Find ("guishell");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(collecteditem == 1)
		{
			InvokeRepeating("FlashMushroom", 0, 1);
		}
		if(collecteditem == 2)
		{
			InvokeRepeating("FlashShell", 5, 1);
		}
		if(collecteditem == 0)
		{

		}
	}

	void FlashMushroom()
	{
		if(guimushroom.activeSelf)
		{
			guimushroom.SetActive (false);
		}
		else
		{
			guimushroom.SetActive (true);
		}
	}

	void FlashShell()
	{
		if(guishell.activeSelf)
		{
			guishell.SetActive (false);
		}
		else
		{
			guishell.SetActive (true);
		}
	}

}

