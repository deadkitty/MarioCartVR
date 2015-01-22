using UnityEngine;
using System.Collections;

public class motor : MonoBehaviour 
{
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject.Find ("enginesound").GetComponent<AudioSource> ().pitch = GameObject.Find ("backleftwheel").GetComponent<WheelCollider> ().rpm / 800;
	}
}
