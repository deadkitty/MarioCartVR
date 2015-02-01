using UnityEngine;
using System.Collections;

public class Turtle : MonoBehaviour 
{
    public GameObject otherPlayer;

    public float distanceToPlayer = 5.0f;
    public float currentTime = 0.0f;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

	void Start () 
    {
        otherPlayer = NetworkManager.sInstance.players[1];

        distanceToPlayer = Vector3.Distance(transform.position, otherPlayer.transform.position);
	}
	
	void Update () 
    {
        if(networkView.isMine)
        {
            currentTime += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, otherPlayer.transform.position, currentTime / distanceToPlayer);
        }
        else
        {

        }
	}

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;

        if (stream.isWriting)
        {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == otherPlayer)
        {
            Destroy(gameObject);
        }
    }
}
