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

    public float yForce = 1000.0f;
    public float sideForce = 200.0f;

    private float xForce;
    private float zForce;

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

        transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);

        rigidbody.AddForce(owner.rigidbody.velocity);

        xForce = Random.Range(-sideForce, sideForce);
        zForce = Random.Range(-sideForce, sideForce);

        lastPosition = transform.position;
	}
    
	void Update ()
    {
        if (networkView.isMine)
        {
            Vector3 lookDir;
            Vector3 enemyDir;
            float angle;

            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

            lookDir = transform.forward;
            lookDir.y = 0.0f;
            
            enemyDir = otherPlayer.transform.position - transform.position;
            enemyDir.y = 0.0f;

            angle = Vector3.Angle(transform.forward, enemyDir);

            if (angle > 5.0f)
            {
                //take a sample position right and left from the turtle shell
                Vector3 posRight = transform.position + transform.right * 0.5f;
                Vector3 posLeft  = transform.position - transform.right * 0.5f;

                //get distance from these points to the otherplayers position
                float distanceRight = Vector3.Distance(posRight, otherPlayer.transform.position);
                float distanceLeft  = Vector3.Distance(posLeft , otherPlayer.transform.position);

                //if rightDistance is lower than rotate turtleshell right
                if (distanceRight < distanceLeft)
                {
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                else //otherwise rotate left
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
            collision.gameObject.networkView.RPC("HitPlayer", RPCMode.AllBuffered, xForce, yForce, zForce);
            
            if (networkView.isMine)
            {
                Network.Destroy(gameObject);
            }
        }
    }
}
