using UnityEngine;
using System.Collections;

public class carcontrols : MonoBehaviour 
{

	float m_maxSpeed = 200.0f;
	float m_wheelrpm = 0.0f;
	float m_currentSpeed = 0;
	float m_wheelangular = 0;

	int m_reachedmaxspeed = 0;
	int m_reachedminspeed = 0;

	WheelCollider frontleftwheel;
	WheelCollider frontrightwheel;
	WheelCollider backleftwheel;
	WheelCollider backrightwheel;
	
	
	void Start () 
	{
		frontleftwheel = GameObject.Find ("frontleftwheel").GetComponent<WheelCollider>();
		frontrightwheel = GameObject.Find ("frontrightwheel").GetComponent<WheelCollider>();
		backleftwheel = GameObject.Find ("backleftwheel").GetComponent<WheelCollider>();
		backrightwheel = GameObject.Find ("backrightwheel").GetComponent<WheelCollider>();
	}

	void Update () 
	{
		//Debug.Log("Speed:" + m_currentSpeed + "RPM:" + frontleftwheel.rpm);
		m_wheelrpm = frontleftwheel.rpm;


		if(Input.GetKey("up"))
		{
			if(m_wheelrpm > 200.0f)
			{
				m_currentSpeed = 0.0f;
			}
			else
			{
				m_currentSpeed += 1.0f;
			}
		}
		if(Input.GetKeyUp("up"))
		{
			m_currentSpeed = 0.0f;
		}
		if(Input.GetKey("down"))
		{
			if(m_wheelrpm < -100.0f)
			{
				m_currentSpeed = 0.0f;
			}
			else
			{
				m_currentSpeed -= 1.0f;
			}

		}
		if(Input.GetKeyUp("down"))
		{
			m_currentSpeed = 0.0f;
		}
		if(Input.GetKey("left") && m_wheelangular > -10.0f)
		{
			m_wheelangular -= 1.0f;
		}
		if(Input.GetKeyUp("left"))
		{
			m_wheelangular = 0.0f;
		}
		if(Input.GetKey("right") && m_wheelangular < 10.0f)
		{
			m_wheelangular += 1.0f;
		}
		if(Input.GetKeyUp("right"))
		{
			m_wheelangular = 0.0f;
		}
		if(Input.GetKey(KeyCode.Return))
		{
			Application.LoadLevel(0);
		}
		if(Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}

		frontleftwheel.motorTorque 	= m_currentSpeed;
		frontrightwheel.motorTorque = m_currentSpeed;
		backleftwheel.motorTorque 	= m_currentSpeed;
		backrightwheel.motorTorque 	= m_currentSpeed;
		frontleftwheel.steerAngle = m_wheelangular;
		frontrightwheel.steerAngle = m_wheelangular;

	}
}