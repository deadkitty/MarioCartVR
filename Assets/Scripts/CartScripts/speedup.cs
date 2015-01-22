using UnityEngine;
using System.Collections;

public class speedup : MonoBehaviour {

	Rigidbody CartModel;
	BoxCollider ItemMesh;

	private int speedupstate = 0;

	// Use this for initialization
	void Start () 
	{
		CartModel = GameObject.Find ("Cart").GetComponent<Rigidbody>();
		ItemMesh  =  GameObject.Find ("Item").GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter(Collider ItemMesh)
	{
		while(speedupstate == 0)
		{
			speedupstate = 1;
			CartModel.AddForce (Vector3.forward * 999999);
		}
	}

	void OnTriggerExit(Collider ItemMesh)
	{
		speedupstate = 0;
	}
}
