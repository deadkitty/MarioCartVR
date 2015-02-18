using UnityEngine;
using System.Collections;

public class helicontrols : MonoBehaviour {
	
	// 0 = EngineStop 1 = EngineStart
	int EngineStatus = 1;
	float startupcounter = 0.0f;
	
	// Continuing Force from Ground
	float UpForce = 100.0f;
	float DownForce = 0.0f;
	float ForwardForce = 0.0f;
	float BackwardForce = 0.0f;
	float LeftForce = 0.0f;
	float RightForce = 0.0f;
	
	// Speed Parameters
	
	public float Speed = 0.0f;
	public float minSpeed = 0.0f;
	public float maxSpeed = 400.0f;
	public float Accel = 3.0f;
	public float decel = 2.0f;
	
	// Experimental Parameters
	
	public float UpSpeed = 0.0f;
	public float UpminSpeed = -1.0f;
	public float UpmaxSpeed = 100.0f;
	public float UpAccel = 5.0f;
	public float Updecel = 3.0f;

	// Horizontal Positioning
	public float HSpeed = 0.0f;
	public float HpminSpeed = -1.0f;
	public float HpmaxSpeed = 100.0f;
	public float HpAccel = 5.0f;
	public float Hpdecel = 3.0f; 
	
	
	// ForceMultiplyer for Helicopter Movement
	public float weight = 0.2f;
	public float MovementForceMultiplyer = 100.0f;
	public float UpForceMultiplyer = 0.0f;
	public float DownForceMultiplyer = 0.0f;
	public float ForwardForceMultiplyer = 0.0f;
	public float BackwardForceMultiplyer = 0.0f;
	public float RightForceMultiplyer = 0.0f;
	public float LeftForceMultiplyer = 0.0f;
	
	// Heli Rotations
	public float m_RotationSpeed = 50.0f;
	
	// Movement
	public float MovementSpeed_UP 			= 60.0f;
	public float MovementSpeed_DOWN 		= 60.0f;
	public float MovementSpeed_FORWARD 		= 60.0f;
	public float MovementSpeed_BACK 		= 60.0f;
	public float MovementSpeed_LEFT 		= 60.0f;
	public float MovementSpeed_RIGHT 		= 60.0f;
	
	// Positioning & Rotation Data
	Vector3 t_position;
	Quaternion t_rotation;
	
	Vector3 TmpYPosition;

	// Camera Mode

	int CamMode = 0; // 0 = First Person / 1 = Third Person
	float CamModeDelay = -1.0f;
	float EngineStatusDelay = -1.0f;
	
	struct SHeliChasisRotation
	{
		float m_RotationX;
		float m_RotationY;
		float m_RotationZ;
	}
	
	
	struct SHeliChasisTransform
	{
		float m_PositionX;
		float m_PositionY;
		float m_PositionZ;
	}
	
	GameObject Helicopter;
	GameObject Mainrotor;
	GameObject OculusRig;
	
	
	Vector3 currentPosition;
	Vector3 targetPosition;
	
	// Use this for initialization
	void Start () 
	{
		SHeliChasisTransform helitransform;
		Helicopter = GameObject.Find ("RASHelicopter");
		Mainrotor = GameObject.Find ("main_rotor");
		OculusRig = GameObject.Find ("OVRCameraRig");
		GameObject.Find ("RotorSounds").GetComponent<AudioSource> ().Play ();
		Speed = 0.0f;
		//t_position = Helicopter.GetComponent<Rigidbody> ().position;
		
		currentPosition = transform.position;
		targetPosition = transform.position;
	}
	
	void Update() 
	{

		if(Input.GetButtonDown ("XBOX_START"))
		{
			if(EngineStatus == 1 && EngineStatusDelay <= 0)
			{
				EngineStatus = 0;
				EngineStatusDelay = 1;
				Helicopter.GetComponentInChildren<Rigidbody>().useGravity = true;
			}

			if(EngineStatus == 0 && EngineStatusDelay <= 0)
			{
				EngineStatus = 1;
				EngineStatusDelay = 1;
				GameObject.Find ("RotorSounds").GetComponent<AudioSource> ().Play ();
				Helicopter.GetComponentInChildren<Rigidbody>().useGravity = false;
			}
		}

		if (EngineStatus == 1) 
		{
			//Debug.Log ("Cam Swtich Delay"+CamModeDelay);
			Debug.Log (startupcounter);

			if (Input.GetKey (KeyCode.Space)) {
				UpSpeed += UpAccel;
			}
			if (Input.GetKey (KeyCode.LeftShift)) {
				UpSpeed -= Updecel;
			}
			if (Input.GetKey (KeyCode.W)) {
				Speed += Accel;
				//Speed += Input.GetAxis("XBOX_Forward")
			}
			if (Input.GetKey (KeyCode.S)) {
				Speed -= decel;
			}
			
			if (Input.GetKey (KeyCode.A)) {
				transform.Rotate (Vector3.up * Time.deltaTime * m_RotationSpeed);
			}
			if (Input.GetKey (KeyCode.D)) {
				transform.Rotate (Vector3.down * Time.deltaTime * m_RotationSpeed);
			}
			// Controls with XBOX Controler
			if (Input.GetAxisRaw ("XBOX_Y_AXIS") > 0) {
				Speed += Input.GetAxisRaw ("XBOX_Y_AXIS");
			}
			if (Input.GetAxisRaw ("XBOX_Y_AXIS") < 0) {
				Speed -= Input.GetAxisRaw ("XBOX_Y_AXIS") * -1;
			}
			if (Input.GetAxisRaw ("XBOX_3rd_AXIS") > 0) {
				UpSpeed += Input.GetAxisRaw ("XBOX_3rd_AXIS");
			}
			if (Input.GetAxisRaw ("XBOX_3rd_AXIS") < 0) {
				UpSpeed -= Input.GetAxisRaw ("XBOX_3rd_AXIS") * -1;
			}
			if (Input.GetAxisRaw ("XBOX_X_AXIS") > 0) {
				HSpeed += Input.GetAxisRaw ("XBOX_X_AXIS");
				//transform.Rotate (Vector3.down * Input.GetAxisRaw ("XBOX_X_AXIS") * m_RotationSpeed);
			}
			if (Input.GetAxisRaw ("XBOX_X_AXIS") < 0) {
				HSpeed -= Input.GetAxisRaw ("XBOX_X_AXIS") * -1;
				//transform.Rotate (Vector3.up * Input.GetAxisRaw ("XBOX_X_AXIS") * -1 * m_RotationSpeed);
			}
			if (Input.GetAxisRaw ("XBOX_X_AXIS_R") > 0) {
				transform.Rotate (Vector3.down * Input.GetAxisRaw ("XBOX_X_AXIS_R") * m_RotationSpeed);
			}
			if (Input.GetAxisRaw ("XBOX_X_AXIS_R") < 0) {
				transform.Rotate (Vector3.up * Input.GetAxisRaw ("XBOX_X_AXIS_R") * -1 * m_RotationSpeed);
			}
			if (Input.GetKey (KeyCode.Return) || Input.GetButtonDown ("XBOX_SELECT")) {

				if (CamMode == 0 && CamModeDelay <= 0.0f) {
					OculusRig.GetComponent<Transform> ().transform.localPosition = new Vector3 (-1.5f, 2.94f, 15.8f);
					CamMode = 1;
					CamModeDelay = 1;
				}
				if (CamMode == 1 && CamModeDelay <= 0.0f) {
					OculusRig.GetComponent<Transform> ().transform.localPosition = new Vector3 (0.0f, 30.37f, -70.5f);
					CamMode = 0;
					CamModeDelay = 1;
				}
			}
			
			Speed *= 0.99f;
			UpSpeed *= 0.99f;
			HSpeed *= 0.99f;
			
			Speed = Mathf.Clamp (Speed, -maxSpeed, maxSpeed);
			UpSpeed = Mathf.Clamp (UpSpeed, -UpmaxSpeed, UpmaxSpeed);
			HSpeed = Mathf.Clamp (HSpeed, -HpmaxSpeed, HpmaxSpeed);
			
			Vector3 newPosition = Vector3.forward * Time.deltaTime * Speed;
			newPosition += Vector3.up * Time.deltaTime * UpSpeed;
			newPosition += Vector3.left * Time.deltaTime * HSpeed;
			
			transform.Translate (newPosition);
			
			float height = Mathf.Clamp (transform.position.y, 20.0f, 300.0f);
			transform.position = new Vector3 (transform.position.x, height, transform.position.z);

			if (CamModeDelay > 0) {
				CamModeDelay -= Time.deltaTime;
			}
		}
		else
		{

		}

		if (EngineStatusDelay > 0) {
			EngineStatusDelay -= Time.deltaTime;
		}

		if(startupcounter < 15.0f && EngineStatus == 1)
		{
			startup();
		}
		if(startupcounter > 0.0f && EngineStatus == 0)
		{
			stopup();
		}
		if(startupcounter > 15.0f)
		{
			helirotor();
		}

	}
	
	void helirotor()
	{
		Mainrotor.GetComponent<Transform>().Rotate(Vector3.forward * Time.deltaTime * 1000);
	}

	void startup()
	{
		startupcounter += Time.deltaTime*3;
		Mainrotor.GetComponent<Transform>().Rotate(Vector3.forward * startupcounter);
		GameObject.Find ("RotorSounds").GetComponent<AudioSource> ().pitch = startupcounter / 15;
	}

	void stopup()
	{
		startupcounter -= Time.deltaTime*3;
		Mainrotor.GetComponent<Transform>().Rotate(Vector3.forward * startupcounter);
		GameObject.Find ("RotorSounds").GetComponent<AudioSource> ().pitch = startupcounter / 15;
	}
	
	/*
	void contupforce()
	{
		Helicopter.GetComponent<Rigidbody>().SetDensity(weight);
		Helicopter.GetComponent<Rigidbody>().AddForce (Vector3.up * UpForce);
		
		//Helicopter.GetComponent<Rigidbody> ().transform.position = HeliPositionForce;
		//Helicopter.GetComponent<Rigidbody>().transform.position = Vector3(BackwardForce, 0, 0);
		
		Debug.Log ("UpForce: " + weight);
	}*/
}
