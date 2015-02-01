using UnityEngine;
using System.Collections;

public class CartMovement : MonoBehaviour
{
    #region Fields
    
    //drive behavior
    public float wheelRadius = 0.7500042f;
    public float suspensionRange = 0.1f;
    public float suspensionDamper = 50.0f;
    public float suspensionSpringFront = 18500.0f;
    public float suspensionSpringRear = 9000.0f;

    public Material brakeLights;

    public Vector3 dragMultiplier = new Vector3(2, 5, 1);

    public float throttle = 0.0f;
    private float steer = 0.0f;
    private bool handbrake = false;

    public Transform centerOfMass;

    public Transform[] frontWheels;
    public Transform[] rearWheels;

    private Wheel[] wheels;

    private WheelFrictionCurve wfc;

    public float topSpeed = 160.0f;
    public int numberOfGears = 5;

    public int maximumTurn = 15;
    public int minimumTurn = 10;

    private float[] engineForceValues;
    private float[] gearSpeeds;

    private int currentGear;
    private float currentEnginePower = 0.0f;

    private float handbrakeXDragFactor = 0.5f;
    private float initialDragMultiplierX = 10.0f;
    private float handbrakeTime = 0.0f;
    private float handbrakeTimer = 1.0f;

    private bool canSteer;
    private bool canDrive;

    //reset car
    private Transform lastCheckpoint;

    public float resetTime = 5.0f;
    private float resetTimer = 0.0f;

    public bool canReset = true;

    //GUI
    private TextMesh SpeedGUI;
    private float m_wheelrpm = 0f;

    //others
    public ItemController itemController;

    public class Wheel
    {
        public WheelCollider collider;
        public Transform wheelGraphic;
        public Transform tireGraphic;
        public bool driveWheel = false;
        public bool steerWheel = false;
        public int lastSkidmark = -1;
        public Vector3 lastEmitPosition = Vector3.zero;
        public float lastEmitTime = Time.time;
        public Vector3 wheelVelo = Vector3.zero;
        public Vector3 groundSpeed = Vector3.zero;
    }
    
    #endregion

    #region Functions Called By Unity

    void Start()
    {
        // For Change the Holo Speed GUI in Game
        SpeedGUI = GetComponentInChildren<TextMesh>();

        if(networkView.isMine)
        {
            SetupCamera();
        }

        SetupWheelColliders();

        SetupCenterOfMass();

        topSpeed = Convert_Miles_Per_Hour_To_Meters_Per_Second(topSpeed);

        SetupGears();

        initialDragMultiplierX = dragMultiplier.x;

        itemController = GetComponent<ItemController>();
    }

    void Update()
    {
        if (networkView.isMine && CartTimer.BeginRace)
        {
            Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);

            GetInput();

            CheckResetTimer();
            
            UpdateWheelGraphics(relativeVelocity);

            UpdateGear(relativeVelocity);
        }
    }

    void FixedUpdate()
    {
        // The rigidbody velocity is always given in world space, but in order to work in local space of the car model we need to transform it first.
        Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);

        CalculateState();

        UpdateFriction(relativeVelocity);

        UpdateDrag(relativeVelocity);

        CalculateEnginePower(relativeVelocity);

        ApplyThrottle(canDrive, relativeVelocity);

        ApplySteering(canSteer, relativeVelocity);
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "Checkpoint")
        {
            lastCheckpoint = collider.transform;
        }
    }

    [RPC]
    void HitPlayer(float xForce, float yForce, float zForce)
    {
        Debug.Log("HitPlayer");

        if (networkView.isMine && !itemController.shieldEnabled)
        {
            rigidbody.AddForce((xForce - rigidbody.velocity.x) * 1000.0f, yForce * 1000.0f, (zForce - rigidbody.velocity.z) * 1000.0f);

            canReset = false;
        }
    }

    #endregion

    #region Functions called from Start()

    void SetupCamera()
    {
        GameObject camera         = GameObject.Find("Camera");
        Transform cameraPosition  = transform.FindChild("CameraPosition");
        camera.transform.position = cameraPosition.position;
        camera.transform.rotation = cameraPosition.rotation;
        camera.transform.parent   = gameObject.transform;
    }

    void SetupWheelColliders()
    {
        SetupWheelFrictionCurve();

        wheels = new Wheel[frontWheels.Length + rearWheels.Length];

        int wheelCount = 0;

        foreach (Transform t in frontWheels)
        {
            wheels[wheelCount] = SetupWheel(t, true);
            wheelCount++;
        }

        foreach (Transform t in rearWheels)
        {
            wheels[wheelCount] = SetupWheel(t, false);
            wheelCount++;
        }
    }

    void SetupWheelFrictionCurve()
    {
        wfc = new WheelFrictionCurve();
        wfc.extremumSlip = 1;
        wfc.extremumValue = 50;
        wfc.asymptoteSlip = 2;
        wfc.asymptoteValue = 25;
        wfc.stiffness = 1;
    }

    Wheel SetupWheel(Transform wheelTransform, bool isFrontWheel)
    {
        GameObject go = new GameObject(wheelTransform.name + " Collider");
        go.transform.position = wheelTransform.position;
        go.transform.parent = transform;
        go.transform.rotation = wheelTransform.rotation;
        go.transform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));

        WheelCollider wc = go.AddComponent(typeof(WheelCollider)) as WheelCollider;
        wc.suspensionDistance = suspensionRange;
        JointSpring js = wc.suspensionSpring;

        if (isFrontWheel)
        {
            js.spring = suspensionSpringFront;
        }
        else
        {
            js.spring = suspensionSpringRear;
        }

        js.damper = suspensionDamper;
        wc.suspensionSpring = js;

        Wheel wheel = new Wheel();
        wheel.collider = wc;
        wc.sidewaysFriction = wfc;
        wheel.wheelGraphic = wheelTransform;
        wheel.tireGraphic = wheelTransform.GetComponentsInChildren<Transform>()[0];

        //wheelRadius = wheel.tireGraphic.renderer.bounds.size.y / 2;	
        wheel.collider.radius = wheelRadius;

        if (isFrontWheel)
        {
            wheel.steerWheel = true;

            go = new GameObject(wheelTransform.name + " Steer Column");
            go.transform.position = wheelTransform.position;
            go.transform.rotation = wheelTransform.rotation;
            go.transform.parent = transform;
            wheelTransform.parent = go.transform;
        }
        else
        {
            wheel.driveWheel = true;
        }

        return wheel;
    }

    void SetupCenterOfMass()
    {
        if (centerOfMass != null)
            rigidbody.centerOfMass = centerOfMass.localPosition;
    }

    void SetupGears()
    {
        engineForceValues = new float[numberOfGears];
        gearSpeeds = new float[numberOfGears];

        float tempTopSpeed = topSpeed;

        for (var i = 0; i < numberOfGears; i++)
        {
            if (i > 0)
                gearSpeeds[i] = tempTopSpeed / 4 + gearSpeeds[i - 1];
            else
                gearSpeeds[i] = tempTopSpeed / 4;

            tempTopSpeed -= tempTopSpeed / 4;
        }

        float engineFactor = topSpeed / gearSpeeds[gearSpeeds.Length - 1];

        for (int i = 0; i < numberOfGears; i++)
        {
            float maxLinearDrag = gearSpeeds[i] * gearSpeeds[i];// * dragMultiplier.z;
            engineForceValues[i] = maxLinearDrag * engineFactor;
        }
    }
    
    #endregion

    #region Functions called from Update()

    void GetInput()
    {
        throttle = 0.0f;

        if(Input.GetButton("Gas"))
        {
            throttle = 1.0f;
        }
        
        if (Input.GetButton("Break"))
        {
            throttle = -1.0f;
        }

        steer = Input.GetAxis("Horizontal");

        if (throttle < 0.0)
        {
            brakeLights.SetFloat("_Intensity", -throttle);
        }
        else
        {
            brakeLights.SetFloat("_Intensity", 0.0f);
        }

        if (Input.GetButton("ResetCar"))
        {
            ResetCar();
        }

        CheckHandbrake();
    }

    void CheckHandbrake()
    {
        if (Input.GetKey("space") || Input.GetButton("Handbrake"))
        {
            if (!handbrake)
            {
                handbrake = true;
                handbrakeTime = Time.time;
                dragMultiplier.x = initialDragMultiplierX * handbrakeXDragFactor;
            }
        }
        else if (handbrake)
        {
            handbrake = false;
            StartCoroutine("StopHandbraking", Mathf.Min(5.0f, Time.time - handbrakeTime));
        }
    }

    void StopHandbraking(float seconds)
    {
        float diff = initialDragMultiplierX - dragMultiplier.x;
        handbrakeTimer = 1;

        // Get the x value of the dragMultiplier back to its initial value in the specified time.
        while (dragMultiplier.x < initialDragMultiplierX && !handbrake)
        {
            dragMultiplier.x += diff * (Time.deltaTime / seconds);
            handbrakeTimer -= Time.deltaTime / seconds;
        }

        dragMultiplier.x = initialDragMultiplierX;
        handbrakeTimer = 0;
    }

    void CheckResetTimer()
    {
        if(!canReset)
        {
            resetTimer += Time.deltaTime;
        }
        else
        {
            resetTimer = 0;
        }

        if (resetTimer > resetTime && !canReset)
        {
            canReset = true;
            ResetCar();
        }
    }

    void ResetCar()
    {
        if(canReset)
        {
            transform.position = lastCheckpoint.position;
            transform.rotation = Quaternion.LookRotation(lastCheckpoint.forward);
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            resetTimer = 0;
            currentEnginePower = 0;
        }
    }

    void UpdateWheelGraphics(Vector3 relativeVelocity)
    {
        float wheelCount = -1;
		m_wheelrpm = 0;

	    foreach (Wheel w in wheels)
        {
            wheelCount++;
            WheelCollider wheel = w.collider;
            WheelHit wh = new WheelHit();

			m_wheelrpm += Mathf.Round(wheel.rpm) / 10;

            // First we get the velocity at the point where the wheel meets the ground, if the wheel is touching the ground
            if (wheel.GetGroundHit(out wh))
            {
                w.wheelGraphic.localPosition = wheel.transform.up * (wheelRadius + wheel.transform.InverseTransformPoint(wh.point).y);
                w.wheelVelo = rigidbody.GetPointVelocity(wh.point);
                w.groundSpeed = w.wheelGraphic.InverseTransformDirection(w.wheelVelo);
            }
            else
            {
                // If the wheel is not touching the ground we set the position of the wheel graphics to
                // the wheel's transform position + the range of the suspension.
                w.wheelGraphic.position = wheel.transform.position + (-wheel.transform.up * suspensionRange);
                if (w.steerWheel)
                    w.wheelVelo *= 0.9f;
                else
                    w.wheelVelo *= 0.9f * (1 - throttle);
            }
            // If the wheel is a steer wheel we apply two rotations:
            // *Rotation around the Steer Column (visualizes the steer direction)
            // *Rotation that visualizes the speed
            if (w.steerWheel)
            {
                Vector3 ea = w.wheelGraphic.parent.localEulerAngles;
                ea.y = steer * maximumTurn;
                w.wheelGraphic.parent.localEulerAngles = ea;
                w.tireGraphic.Rotate(Vector3.right * (w.groundSpeed.z / wheelRadius) * Time.deltaTime * Mathf.Rad2Deg);
            }
            else if (!handbrake && w.driveWheel)
            {
                // If the wheel is a drive wheel it only gets the rotation that visualizes speed.
                // If we are hand braking we don't rotate it.
                w.tireGraphic.Rotate(Vector3.right * (w.groundSpeed.z / wheelRadius) * Time.deltaTime * Mathf.Rad2Deg);
            }
        }
		m_wheelrpm /= wheels.Length;

		SpeedGUI.text = m_wheelrpm.ToString() + " KM/h";

    }

    void UpdateGear(Vector3 relativeVelocity)
    {
        currentGear = 0;
        for (var i = 0; i < numberOfGears - 1; i++)
        {
            if (relativeVelocity.z > gearSpeeds[i])
                currentGear = i + 1;
        }
    }
    
    #endregion
    
    #region Functions called from FixedUpdate()

    void UpdateDrag(Vector3 relativeVelocity)
    {
        Vector3 relativeDrag = new Vector3(-relativeVelocity.x * Mathf.Abs(relativeVelocity.x),
                                            -relativeVelocity.y * Mathf.Abs(relativeVelocity.y),
                                            -relativeVelocity.z * Mathf.Abs(relativeVelocity.z));

        Vector3 drag = Vector3.Scale(dragMultiplier, relativeDrag);

        if (initialDragMultiplierX > dragMultiplier.x) // Handbrake code
        {
            drag.x /= (relativeVelocity.magnitude / (topSpeed / (1 + 2 * handbrakeXDragFactor)));
            drag.z *= (1 + Mathf.Abs(Vector3.Dot(rigidbody.velocity.normalized, transform.forward)));
            drag += rigidbody.velocity * Mathf.Clamp01(rigidbody.velocity.magnitude / topSpeed);
        }
        else // No handbrake
        {
            drag.x *= topSpeed / relativeVelocity.magnitude;
        }

        if (Mathf.Abs(relativeVelocity.x) < 5 && !handbrake)
            drag.x = -relativeVelocity.x * dragMultiplier.x;

        rigidbody.AddForce(transform.TransformDirection(drag) * rigidbody.mass * Time.deltaTime);
    }

    void UpdateFriction(Vector3 relativeVelocity)
    {
        float sqrVel = relativeVelocity.x * relativeVelocity.x;

        // Add extra sideways friction based on the car's turning velocity to avoid slipping
        wfc.extremumValue = Mathf.Clamp(300 - sqrVel, 0, 300);
        wfc.asymptoteValue = Mathf.Clamp(150 - (sqrVel / 2), 0, 150);

        foreach (Wheel w in wheels)
        {
            w.collider.sidewaysFriction = wfc;
            w.collider.forwardFriction = wfc;
        }
    }

    void CalculateEnginePower(Vector3 relativeVelocity)
    {
        if (throttle == 0)
        {
            currentEnginePower -= Time.deltaTime * 200;
        }
        else if (HaveTheSameSign(relativeVelocity.z, throttle))
        {
            float normPower = (currentEnginePower / engineForceValues[engineForceValues.Length - 1]) * 2;
            currentEnginePower += Time.deltaTime * 200 * EvaluateNormPower(normPower);
        }
        else
        {
            currentEnginePower -= Time.deltaTime * 300;
        }

        if (currentGear == 0)
            currentEnginePower = Mathf.Clamp(currentEnginePower, 0, engineForceValues[0]);
        else
            currentEnginePower = Mathf.Clamp(currentEnginePower, engineForceValues[currentGear - 1], engineForceValues[currentGear]);
    }

    void CalculateState()
    {
        canDrive = false;
        canSteer = false;

        foreach (Wheel w in wheels)
        {
            if (w.collider.isGrounded)
            {
                if (w.steerWheel)
                    canSteer = true;
                if (w.driveWheel)
                    canDrive = true;
            }
        }
    }

    void ApplyThrottle(bool canDrive, Vector3 relativeVelocity)
    {
        if (canDrive)
        {
            float throttleForce = 0;
            float brakeForce = 0;

            if (HaveTheSameSign(relativeVelocity.z, throttle))
            {
                if (!handbrake)
                    throttleForce = Mathf.Sign(throttle) * currentEnginePower * rigidbody.mass;
            }
            else
                brakeForce = Mathf.Sign(throttle) * engineForceValues[0] * rigidbody.mass;

            rigidbody.AddForce(transform.forward * Time.deltaTime * (throttleForce + brakeForce));
        }
    }

    void ApplySteering(bool canSteer, Vector3 relativeVelocity)
    {
        if (canSteer)
        {
            float turnRadius = 3.0f / Mathf.Sin((90.0f - (steer * 30.0f)) * Mathf.Deg2Rad);
            float minMaxTurn = EvaluateSpeedToTurn(rigidbody.velocity.magnitude);
            float turnSpeed = Mathf.Clamp(relativeVelocity.z / turnRadius, -minMaxTurn / 10, minMaxTurn / 10);

            transform.RotateAround(transform.position + transform.right * turnRadius * steer,
                                    transform.up,
                                    turnSpeed * Mathf.Rad2Deg * Time.deltaTime * steer);

            var debugStartPoint = transform.position + transform.right * turnRadius * steer;
            var debugEndPoint = debugStartPoint + Vector3.up * 5;

            Debug.DrawLine(debugStartPoint, debugEndPoint, Color.red);

            if (initialDragMultiplierX > dragMultiplier.x) // Handbrake
            {
                // rotationDirection is -1 or 1 by default, depending on steering
                float rotationDirection = Mathf.Sign(steer);
                if (steer == 0)
                {
                    // If we are not steering and we are handbraking and not rotating fast, we apply a random rotationDirection
                    if (rigidbody.angularVelocity.y < 1)
                    {
                        rotationDirection = Random.Range(-1.0f, 1.0f);
                    }
                    else
                    {
                        // If we are rotating fast we are applying that rotation to the car
                        rotationDirection = rigidbody.angularVelocity.y;
                    }
                }
                // -- Finally we apply this rotation around a point between the cars front wheels.
                transform.RotateAround(
                    transform.TransformPoint((frontWheels[0].localPosition + frontWheels[1].localPosition) * 0.5f),
                                              transform.up,
                                              rigidbody.velocity.magnitude * Mathf.Clamp01(1 - rigidbody.velocity.magnitude / topSpeed) * rotationDirection * Time.deltaTime * 2);
            }
        }
    }
    
    #endregion

    #region Util

    float Convert_Miles_Per_Hour_To_Meters_Per_Second(float value)
    {
        return value * 0.44704f;
    }

    float Convert_Meters_Per_Second_To_Miles_Per_Hour(float value)
    {
        return value * 2.23693629f;
    }

    bool HaveTheSameSign(float first, float second)
    {
        return Mathf.Sign(first) == Mathf.Sign(second);
    }

    float EvaluateSpeedToTurn(float speed)
    {
        if (speed > topSpeed * 0.5f)
            return minimumTurn;

        float speedIndex = 1.0f - (speed / (topSpeed * 0.5f));
        return minimumTurn + speedIndex * (maximumTurn - minimumTurn);
    }

    float EvaluateNormPower(float normPower)
    {
        if (normPower < 1.0f)
            return 10.0f - normPower * 9.0f;
        else
            return 1.9f - normPower * 0.9f;
    }

    float GetGearState()
    {
        Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        float lowLimit = currentGear == 0 ? 0.0f : gearSpeeds[currentGear - 1];
        return (relativeVelocity.z - lowLimit) / (gearSpeeds[currentGear - (int)lowLimit]) * (1 - currentGear * 0.1f) + currentGear * 0.1f;
    }
    
    #endregion
}
