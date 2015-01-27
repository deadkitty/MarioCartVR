using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour 
{
    public enum EItemType
    {
        mushroom =  0, //cart acceleration
        shield   =  1, //evade fire from another cart
        turtle   =  2, //fire on another cart
        count    =  3,
        none     = -1,
    }

    public float respawnTime = 15.0f;
    private float currentTime = 0.0f;

    private bool disabled = false;

    private BoxCollider itemCollider;
    private MeshRenderer meshRenderer;

#if UNITY_EDITOR
    
    public bool useDebugItem = false;
    public EItemType debugItem;

#endif

	void Start () 
    {
        itemCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
	}
	
	void Update ()
    {
	    if(disabled)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= respawnTime)
            {
                EnableItem();
            }
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        EItemType item = (EItemType)Random.Range(0, (int)EItemType.count);

#if UNITY_EDITOR

        switch (item)
        {
            case EItemType.mushroom : Debug.Log("mushroom"); break;
            case EItemType.shield   : Debug.Log("shield"); break;
            case EItemType.turtle   : Debug.Log("turtle"); break;
        }

        if (useDebugItem)
        {
            item = debugItem;
        }

#endif

        ItemController player = collider.gameObject.GetComponent<ItemController>();

        player.currentItem = item;
        networkView.RPC("DisableItem", RPCMode.AllBuffered);
        //if(player.currentItem == EItemType.none)
        //{

        //}
    }

    [RPC]
    void DisableItem()
    {
        Debug.Log("RPC.Item.DisableItem");

        disabled = true;

        meshRenderer.enabled = false;
        itemCollider.enabled = false;
    }

    void EnableItem()
    {
        Debug.Log("Item.EnableItem");

        disabled = false;

        meshRenderer.enabled = true;
        itemCollider.enabled = true;

        currentTime = 0.0f;
    }
}
