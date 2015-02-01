using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public GameObject turtleGameObject;
    public GameObject shieldGameObject;

    public Item.EItemType currentItem = Item.EItemType.none;

    public bool shieldEnabled = false;
    public float shieldResetTime = 10.0f;
    public float shieldResetTimer = 0.0f;

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

        shieldGameObject = transform.FindChild("Shield").gameObject;
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

        ResetShieldTimer();
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
        networkView.RPC("UseShieldRPC", RPCMode.AllBuffered);
    }

    [RPC]
    void UseShieldRPC()
    {
        shieldEnabled = true;
        shieldGameObject.renderer.enabled = true;
    }

    void ResetShieldTimer()
    {
        if(shieldEnabled)
        {
            shieldResetTimer += Time.deltaTime;
        }

        if(shieldResetTimer >= shieldResetTime)
        {
            shieldEnabled = false;
            shieldResetTimer = 0.0f;
            shieldGameObject.renderer.enabled = false;
        }
    }

    void UseTurtle()
    {
        Vector3 turtlePosition = transform.FindChild("ItemSpawn").position;
        Network.Instantiate(turtleGameObject, turtlePosition, transform.rotation, 0);
    }
}
