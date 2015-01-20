using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour 
{
    public float sensitivityX = 15.0f;
    public float sensitivityY = 15.0f;
        
    float rotationY = 0.0f;

	void Start () 
    {

	}
	
	void Update () 
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
	}
}
