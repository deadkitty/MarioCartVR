using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public GameObject turtle;

    public Item.EItemType currentItem = Item.EItemType.none;

    void Start()
    {

    }

    void Update()
    {
        if(networkView.isMine && CartTimer.BeginRace)
        {
            if (Input.GetButton("Fire1"))
            {
                UseItem();
            }
        }
    }

    void UseItem()
    {
        Debug.Log("Player.UseItem");

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
        Vector3 turtlePosition = transform.position;
        
        Network.Instantiate(turtle, turtlePosition + new Vector3(0.0f, 5.0f, 0.0f), Quaternion.identity, 0);

        Turtle turtleScript = turtle.GetComponent<Turtle>();
        
        if(Network.isClient)
        {

        }
    }
}
