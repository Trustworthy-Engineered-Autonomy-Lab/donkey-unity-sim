using UnityEngine;
using System.Collections;
using PathCreation.Examples;

public class Car : MonoBehaviour, ICar{

	public CarSpawner carSpawner;
	public WheelCollider[] wheelColliders;
	public Transform[] wheelMeshes;

	public float maxSpeed = 30f;
	public float maxTorque = 50f;
	public float maxBreakTorque = 50f;
	public AnimationCurve torqueCurve;

	public Transform centrOfMass;

	public float requestTorque = 0f;
	public float requestBrake = 0f;
	public float requestSteering = 0f;

	public Vector3 acceleration = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 prevVel = Vector3.zero;

	public Vector3 startPos;
	public Quaternion startRot;
	private Quaternion rotation = Quaternion.identity;
	private Vector3 gyro = Vector3.zero;

	public centerLine_Distance centerLine_Distance;
	public Rigidbody rb;

	//for logging
	public float lastSteer = 0.0f;
	public float lastAccel = 0.0f;

	//when the car is doing multiple things, we sometimes want to sort out parts of the training
	//use this label to pull partial training samples from a run 
	public string activity = "keep_lane";

    public float maxSteer = 16.0f;

	//name of the last object we hit.
	public string last_collision = "none";
	public string curr_collision = "none";


	public float trackAngle = 0f;
	private float originalMass = -1f;
	private float[] wheelFrictionScales;

	// Asymmetric drag: a constant force applied at the front-right wheel each physics tick
	private float dragForce = 0f;
	private bool applyAsymmetricDrag = false;

	 
	// Use this for initialization
	void Awake () 
	{
		rb = GetComponent<Rigidbody>();
		originalMass = rb.mass; 
		 
		if(rb && centrOfMass)
		{
			rb.centerOfMass = centrOfMass.localPosition;
		}

        // Initialize the centerLine_Distance variable
        centerLine_Distance = GetComponent<centerLine_Distance>();

        // Check if the object is null
        if (centerLine_Distance == null)
        {
            // Create a new object
            centerLine_Distance = new centerLine_Distance();
        }

		requestTorque = 0f;
		requestSteering = 0f;
		wheelFrictionScales = new float[wheelColliders.Length];
		for (int i = 0; i < wheelFrictionScales.Length; i++)
		{
			wheelFrictionScales[i] = 1.0f;
		}

		SavePosRot();
		
		// had to disable this because PID max steering was affecting the global max_steering
        // maxSteer = PlayerPrefs.GetFloat("max_steer", 16.0f);
	}

	public void SavePosRot()
	{
		startPos = transform.position;
		startRot = transform.rotation;
	}

	public void RestorePosRot()
	{
		Set(startPos, startRot);
	}

	public void RequestThrottle(float val)
	{
		requestTorque = val;
		requestBrake = 0f;
		//Debug.Log("request throttle: " + val);
	}

    public void SetMaxSteering(float val)
    {
        maxSteer = val;
		// had to disable this because PID max steering was affecting the global max_steering
        // PlayerPrefs.SetFloat("max_steer", maxSteer);
        // PlayerPrefs.Save();
    }

    public float GetMaxSteering()
    {
        return maxSteer;
    }

	public void RequestSteering(float val)
	{
		requestSteering = Mathf.Clamp(val, -maxSteer, maxSteer);
		//Debug.Log("request steering: " + val);
	}

	public void Set(Vector3 pos, Quaternion rot)
	{
		rb.position = pos;
		rb.rotation = rot;

		//just setting it once doesn't seem to work. Try setting it multiple times..
		StartCoroutine(KeepSetting(pos, rot, 1));
	}

	IEnumerator KeepSetting(Vector3 pos, Quaternion rot, int numIter)
	{
		while(numIter > 0)
		{
			rb.isKinematic = true;
			
			yield return new WaitForFixedUpdate();

			rb.position = pos;
			rb.rotation = rot;
			transform.position = pos;
			transform.rotation = rot;

			numIter--;

			rb.isKinematic = false;
		}
	}

	public float GetSteering()
	{
		return requestSteering;
	}

	public float GetTrackAngle()
	{
		return trackAngle;
	}

	public float GetThrottle()
	{
		return requestTorque;
	}

	public float GetFootBrake()
	{
		return requestBrake;
	}

	public float GetHandBrake()
	{
		return 0.0f;
	}

	public Vector3 GetVelocity()
	{
		return velocity;
	}

	public Vector3 GetAccel()
	{
		return acceleration;
	}
	public Vector3 GetGyro()
	{
	  return gyro;
  	}
	public float GetOrient ()
	{
		Vector3 dir = transform.forward;
		return Mathf.Atan2( dir.z, dir.x);
	}

	public Transform GetTransform()
	{
		return this.transform;
	}

	public bool IsStill()
	{
		return rb.IsSleeping();
	}

	public void RequestFootBrake(float val)
	{
		requestBrake = val;
	}

	public void RequestHandBrake(float val)
	{
		//todo
	}
	
	// Update is called once per frame
	void Update () {
	
		UpdateWheelPositions();
	}

	public string GetActivity()
	{
		return activity;
	}

	public void SetActivity(string act)
	{
		activity = act;
	}

	void FixedUpdate()
	{
		lastSteer = requestSteering;
		lastAccel = requestTorque;
		prevVel = velocity;
		velocity = transform.InverseTransformDirection(rb.velocity);
		acceleration = (velocity - prevVel)/Time.deltaTime;
		gyro = rb.angularVelocity;
		rotation = rb.rotation;
		trackAngle = centerLine_Distance.cAngle;     //This line works and gets the correct value
		
		// use the torque curve
		float throttle = torqueCurve.Evaluate(velocity.magnitude / maxSpeed) * requestTorque * maxTorque;
		float steerAngle = requestSteering;
        float brake = requestBrake * maxBreakTorque;

		//front two tires.
		wheelColliders[2].steerAngle = steerAngle;
		wheelColliders[3].steerAngle = steerAngle;

		// Four wheel drive. Low-friction wheels receive modestly reduced usable
		// drive torque; values above 1 do not boost. Do not add brake here,
		// since very low single-wheel friction should destabilize, not pin the car.
		for(int i = 0; i < wheelColliders.Length; i++)
		{
			WheelCollider wc = wheelColliders[i];
			float frictionScale = Mathf.Clamp01(GetWheelFrictionScale(i));
			float torqueScale = Mathf.Lerp(0.5f, 1.0f, frictionScale);
			wc.motorTorque = throttle * torqueScale;
			wc.brakeTorque = brake;
		}

		// Apply asymmetric drag at the front-right wheel position
		// A backward force at one wheel simulates extra rolling resistance
		// (flat tire, low pressure). Because it's off-center from the CoM,
		// it creates a yaw torque that pulls the car toward that wheel.
		if (applyAsymmetricDrag && rb != null)
		{
			float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
			if (Mathf.Abs(forwardSpeed) > 0.1f)
			{
				Vector3 wheelWorldPos;
				Quaternion wheelWorldRot;
				wheelColliders[2].GetWorldPose(out wheelWorldPos, out wheelWorldRot); // [2] = tireColliderFR

				// Force opposes direction of motion in both forward and reverse
				float dragDirection = -Mathf.Sign(forwardSpeed);
				Vector3 localForce = new Vector3(0f, 0f, dragDirection * dragForce);
				Vector3 worldForce = transform.TransformDirection(localForce);

				rb.AddForceAtPosition(worldForce, wheelWorldPos, ForceMode.Force);
			}
		}

	}

	void FlipUpright()
	{
		Quaternion rot = Quaternion.Euler(180f, 0f, 0f);
		this.transform.rotation = transform.rotation * rot;
		transform.position = transform.position + Vector3.up * 2;
	}

	void UpdateWheelPositions()
	{
		Quaternion rot;
		Vector3 pos;

		for(int i = 0; i < wheelColliders.Length; i++)
		{
			WheelCollider wc = wheelColliders[i];
			Transform tm = wheelMeshes[i];

			wc.GetWorldPose(out pos, out rot);

			tm.position = pos;
			tm.rotation = rot;
		}
	}

	//get the name of the last object we collided with
	public string GetLastCollision()
	{
		return last_collision;
	}

	public string GetCurrentCollision()
	{
    	return curr_collision;
	}

	public void ClearLastCollision()
	{		
		last_collision = "none";
	}
	public void SetMassScale (float scale)
	{
		if(rb != null && originalMass > 0)
		{
			rb.mass = originalMass*scale;
			Debug.Log("Car mass set to " + rb.mass + " (scale: " + scale + ")");
		}
	}

	public void SetDragForce(float force)
	{
		dragForce = force;
		applyAsymmetricDrag = (force != 0f);
		Debug.Log("Asymmetric drag force set to " + force);
	}

	public void SetFrictionScale(float scale) //added for friction loss
	{
		for(int i = 0; i < wheelColliders.Length; i++)
		{
			WheelCollider wc = wheelColliders[i];
			wheelFrictionScales[i] = Mathf.Max(0.0f, scale);
			WheelPhys wp = wc.GetComponent<WheelPhys>();
			if (wp != null)
			{
				wp.SetFrictionScale(scale);
			}
		}
		Debug.Log("Friction scale set to " + scale);
	}

	public void SetWheelFrictionScale(int wheelIndex, float scale) //Added for single wheel friction loss
	{
		if (wheelIndex < 0 || wheelIndex >= wheelColliders.Length)
			return;

		wheelFrictionScales[wheelIndex] = Mathf.Max(0.0f, scale);
		WheelPhys wp = wheelColliders[wheelIndex].GetComponent<WheelPhys>();
		if (wp != null)
		{
			wp.SetFrictionScale(scale);
		}

		Debug.Log("Wheel " + wheelIndex + " friction scale set to " + scale);
	}

	float GetWheelFrictionScale(int wheelIndex)
	{
		if (wheelFrictionScales == null || wheelIndex < 0 || wheelIndex >= wheelFrictionScales.Length)
		{
			return 1.0f;
		}
		return wheelFrictionScales[wheelIndex];
	}

	void OnCollisionEnter(Collision col)
	{
		last_collision = col.gameObject.name;
		curr_collision = col.gameObject.name;
	}

	void OnCollisionStay(Collision col)
	{
    	curr_collision = col.gameObject.name;
	}

	void OnCollisionExit(Collision col)
	{
    	if (curr_collision == col.gameObject.name)
    	{
        	curr_collision = "none";
    	}
	}
}
