using UnityEngine;
using System.Collections;



public class HoloGui : MonoBehaviour {

	TextMesh SpeedGUI;
	WheelCollider frontleftwheel;

	float m_wheelrpm = 0f;

	// Use this for initialization
	void Start () 
	{
		SpeedGUI = GameObject.Find ("HoloGUI").GetComponent<TextMesh> ();
		frontleftwheel = GameObject.Find ("frontleftwheel").GetComponent<WheelCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_wheelrpm = Mathf.Round(frontleftwheel.rpm) / 10;
		SpeedGUI.text = m_wheelrpm.ToString() + " KM/h";
	}
}
