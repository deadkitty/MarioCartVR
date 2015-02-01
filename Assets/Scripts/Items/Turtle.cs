using UnityEngine;
using System.Collections;

public class Turtle : MonoBehaviour 
{
    public GameObject otherPlayer;
    public GameObject owner;

    public float speed = 20.0f;
    public float rotationSpeed = 5.0f;
    
    private Vector3 lastPosition;

    private int cartLayer;

	void Start () 
    {
        cartLayer = LayerMask.NameToLayer("Cart");

        if(networkView.isMine)
        {
            owner = Players.GetPlayer(0);
            otherPlayer = Players.GetPlayer(1);
        }
        else
        {
            otherPlayer = Players.GetPlayer(0);
            owner = Players.GetPlayer(1);
        }

        lastPosition = transform.position;
	}

    public float angle;

	void Update ()
    {
        if (networkView.isMine)
        {
            Vector3 lookDir;
            Vector3 enemyDir;

            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

            lookDir = transform.forward;
            lookDir.y = 0.0f;
            
            enemyDir = otherPlayer.transform.position - transform.position;
            enemyDir.y = 0.0f;

            angle = Vector3.Angle(transform.forward, enemyDir);

            if (angle > 5.0f)
            {
                Vector3 posRight = transform.position + transform.right * 0.5f;
                Vector3 posLeft  = transform.position - transform.right * 0.5f;

                float distanceRight = Vector3.Distance(posRight, otherPlayer.transform.position);
                float distanceLeft  = Vector3.Distance(posLeft , otherPlayer.transform.position);

                //rotate right
                if (distanceRight < distanceLeft)
                {
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                else //rotate left
                {
                    transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                }
            }
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("TurtleCollision " + collision.gameObject.tag);

        if (collision.gameObject.layer == cartLayer && collision.gameObject != owner)
        {
            collision.gameObject.networkView.RPC("HitPlayer", RPCMode.AllBuffered);
            
            if (networkView.isMine)
            {
                Network.Destroy(gameObject);
            }
        }
    }
}
