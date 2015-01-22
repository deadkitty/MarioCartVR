using UnityEngine;
using System.Collections;

public class cartpositioning : MonoBehaviour {

	Rigidbody CartModel;
	
	// Use this for initialization
	void Start () 
	{
		CartModel = GameObject.Find ("Cart").GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CartModel.AddForce (Vector3.down * 30000);
		//CartModel.AddForce (Vector3.back * 100);// DownForce for prevent the hard y rotation by went left or right
	}
}
