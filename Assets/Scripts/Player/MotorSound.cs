using UnityEngine;
using System.Collections;

public class MotorSound : MonoBehaviour 
{
    private AudioSource source;
    private WheelCollider wheelCollider;

    public int value = 800;

	void Start () 
    {
        source = GameObject.Find("enginesound").GetComponent<AudioSource>();
        wheelCollider = GameObject.Find("backleftwheel").GetComponent<WheelCollider>();
	}
	
	void Update ()
    {
        if (networkView.isMine)
        {
            source.pitch = wheelCollider.rpm / value;
        }
	}
}
