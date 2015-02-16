using UnityEngine;
using System.Collections;

public class HeliMovement : MonoBehaviour 
{
    public float speed = 10.0f;
    public float rotationSpeed = 5.0f;

    void Start () 
    {
        SetupCamera();
	}
	
    void Update () 
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, speed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -speed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0.0f, -rotationSpeed * Time.deltaTime, 0.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            transform.Translate(new Vector3(0.0f, -speed * Time.deltaTime, 0.0f));
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(new Vector3(0.0f, speed * Time.deltaTime, 0.0f));
        }
	}


    void SetupCamera()
    {
        GameObject camera = GameObject.Find("Camera");
        Transform cameraPosition = transform.FindChild("CameraPosition");
        camera.transform.position = cameraPosition.position;
        camera.transform.rotation = cameraPosition.rotation;
        camera.transform.parent = gameObject.transform;
    }
}
