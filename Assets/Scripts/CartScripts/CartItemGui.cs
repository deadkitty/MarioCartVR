using UnityEngine;
using System.Collections;

public class CartItemGui : MonoBehaviour 
{

	public int collecteditem;
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

	}

	int cartitemgui (int collecteditem)
	{

		if (collecteditem == 1) 
		{
			 
		}
		if (collecteditem == 1) 
		{
		}
		
		int itemid = 0;
		
		return itemid;
	}

	void guiitemblink(int collecteditem)
	{
		item.SetActive (true);
		yield return new WaitForSeconds(.1f);
		item.SetActive (false);
		yield return new WaitForSeconds(.1f);
	}


}
