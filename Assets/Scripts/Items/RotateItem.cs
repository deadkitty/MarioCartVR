using UnityEngine;
using System.Collections;

public class RotateItem : MonoBehaviour 
{
    public float rotationY = 0.0f;

	void Update () 
    {
        //rotationY += ;
        transform.Rotate(new Vector3(0.0f, -1.0f, 0.0f), Time.deltaTime * 25.0f);
	}
}
