using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    Vector3 position = Vector3.zero;

    public float speed = 75f;
    public float rotationSpeed = 250.0f;

    public bool beginRace = false;

    public bool BeginRace
    {
        set { beginRace = value; }
        get { return beginRace; }
    }

    public GameObject turtle;

    public Item.EItemType currentItem = Item.EItemType.undefined;

    void Start()
    {
        Debug.Log("Player.Start");
        //if(networkView.isMine)
        //{
        //    CartTimer.sInstance.player = this;
        //    NetworkManager.sInstance.players[0] = gameObject;
        //}
        //else
        //{
        //    NetworkManager.sInstance.players[1] = gameObject;
        //}
    }

    void Update()
    {
        if(networkView.isMine && beginRace)
        {
            //float speedMultiplier = Input.GetAxis("Vertical");
            //position.z = speedMultiplier * speed;

            //float rotation = transform.localEulerAngles.y + Input.GetAxis("Horizontal") * rotationSpeed * speedMultiplier * Time.deltaTime;

            //transform.localEulerAngles = new Vector3(0.0f, rotation, 0.0f);

            //transform.Translate(position * Time.deltaTime);

            //rigidbody.AddForce(new Vector3(0.0f, Time.deltaTime * rigidbody.mass, 0.0f));

            //if (Input.GetButton("Fire1"))
            //{
            //    UseItem();
            //}
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

        currentItem = Item.EItemType.undefined;
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
