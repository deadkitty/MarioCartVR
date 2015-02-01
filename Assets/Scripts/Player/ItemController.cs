using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public GameObject turtleGameObject;

    public Item.EItemType currentItem = Item.EItemType.none;
    
    void Start()
    {
        if(networkView.isMine)
        {
            Players.SetPlayer(0, gameObject);
        }
        else
        {
            Players.SetPlayer(1, gameObject);
        }
    }

    void Update()
    {
        if(networkView.isMine && CartTimer.BeginRace)
        {
            if (Input.GetButtonDown("UseItem"))
            {
                UseItem();
            }
        }
    }

    void UseItem()
    {
        switch(currentItem)
        {
            case Item.EItemType.mushroom: UseMushroom(); break;
            case Item.EItemType.shield  : UseShield(); break;
            case Item.EItemType.turtle  : UseTurtle(); break;
        }

        currentItem = Item.EItemType.none;
    }

    void UseMushroom()
    {

    }

    void UseShield()
    {
        
    }

    void UseTurtle()
    {
        Vector3 turtlePosition = transform.FindChild("ItemSpawn").position;
        Network.Instantiate(turtleGameObject, turtlePosition, transform.rotation, 0);
    }
}
