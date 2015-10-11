using System.Collections;
﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlightController : MonoBehaviour
{
	[Header("Control Influence")]
	public float RollModifier = 1.0f;
	public float PitchModifier = 1.0f;
	public float YawModifier = 0.1f;

	[Header("Rotational Control")]
	public float BankingModifier = 0.5f;
	public float RollImpliedPitch = 0.5f;
	public float AutoRollCorrect = 0.2f;
	public float AutoPitchCorrect = 0.2f;

	[Header("Speed Variation")]
	public float EngineForce = 40.0f;
	public float ThrottleInfluence = 0.3f;
	public float AirbrakeMultiplier = 3f;
	public float VelocityDragFactor = 0.001f;

	[Header("Aerodynamic Simulation")]
	public float Lift = 0.002f;
	public float LiftCutoffSpeed = 300.0f;
	public float AerodynamicInfluence = 0.02f;
	public bool AerodynamicCorrection = true;

	[Header("Input Names")]
	public string RollInputName = "Horizontal";
	public string PitchInputName = "Vertical";
	public string YawInputName;
	public string ThrustInputName = "Trigger Right";
	public string BrakeInputName = "Trigger Left";

	private float m_RollInput;
	private float m_PitchInput;
	private float m_YawInput;
	private Vector2 m_SpeedControlInput = Vector3.zero;

	protected Rigidbody m_Rigidbody;
	private float m_OriginalDrag;
	private float m_OriginalAngularDrag;
	private float m_CurrentPitch;
	private float m_CurrentRoll;
	private float m_BankingInfluence;
	private float m_ForwardSpeed;
	private float m_EnginePower;
	private float m_Throttle;
	private float m_AirBrakeImpact;
	private float m_AeroCorrectionFactor;

	public virtual void Awake()
	{
		m_Rigidbody = this.GetComponent<Rigidbody>();
	}

	public virtual void Start()
	{

		m_OriginalDrag = m_Rigidbody.drag;
		m_OriginalAngularDrag = m_Rigidbody.angularDrag;
	}

	public virtual void Update()
	{
		updateInputAngles();
	}

	public virtual void FixedUpdate()
	{
		applyForces();
	}

	protected void updateInputAngles()
	{
		// Poll and store inputs.
		m_RollInput = !System.String.IsNullOrEmpty(RollInputName) ? Input.GetAxis(RollInputName) : 0.0f;
		m_PitchInput = !System.String.IsNullOrEmpty(PitchInputName) ? Input.GetAxis(PitchInputName) : 0.0f;
		m_YawInput = !System.String.IsNullOrEmpty(YawInputName) ? Input.GetAxis(YawInputName) : 0.0f;

		m_SpeedControlInput = new Vector2(!System.String.IsNullOrEmpty(ThrustInputName) ? Input.GetAxis(ThrustInputName) : 0.0f,
		                                  !System.String.IsNullOrEmpty(BrakeInputName) ? Input.GetAxis(BrakeInputName) : 0.0f);

		// Calculate current angles of pitch and roll.
		Vector3 flat = transform.forward;
		flat.y = 0.0f;

		if(flat.sqrMagnitude > 0.0f)
		{
			flat.Normalize();
			Vector3 local_flat = transform.InverseTransformDirection(flat);
			Vector3 local_right = transform.InverseTransformDirection(Vector3.Cross(Vector3.up, flat));

			m_CurrentPitch = Mathf.Atan2(local_flat.y, local_flat.z);
			m_CurrentRoll = Mathf.Atan2(local_right.y, local_right.x);
		}

		// Setup autolevelling, to keep craft from being impossible to control.
		m_BankingInfluence = Mathf.Sin(m_CurrentRoll);

		if(m_RollInput == 0.0f) {
			m_RollInput = -m_CurrentRoll*AutoRollCorrect;
		}

		if(m_PitchInput == 0.0f) {
			m_PitchInput = -m_CurrentPitch*AutoPitchCorrect - Mathf.Abs(m_BankingInfluence*m_BankingInfluence*RollImpliedPitch);
		}

		// Update the player's forward velocity for drag and force reference, ignores backwards velocity.
		m_ForwardSpeed = Mathf.Max(0.0f, transform.InverseTransformDirection(m_Rigidbody.velocity).z);

		// Apply users throttle.
		m_Throttle = Mathf.Clamp01(m_Throttle + m_SpeedControlInput.x*ThrottleInfluence*Time.deltaTime);
		m_EnginePower = EngineForce*m_Throttle;
	}

	protected void applyForces()
	{
		// Imply extra drag from speed.
		m_Rigidbody.drag = (m_OriginalDrag + m_Rigidbody.velocity.magnitude*VelocityDragFactor)*(1.0f + m_SpeedControlInput.y*AirbrakeMultiplier);
		m_Rigidbody.angularDrag = m_OriginalAngularDrag*m_ForwardSpeed;

		// Calculate aerodynamic correction simulation.
		if(AerodynamicCorrection && m_Rigidbody.velocity.sqrMagnitude > 0.0f) {
			m_AeroCorrectionFactor = Vector3.Dot(transform.forward, m_Rigidbody.velocity.normalized);
			m_AeroCorrectionFactor *= m_AeroCorrectionFactor;

			m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, transform.forward*m_ForwardSpeed, AerodynamicInfluence*m_ForwardSpeed*Time.deltaTime);
			m_Rigidbody.rotation = Quaternion.Slerp(m_Rigidbody.rotation, Quaternion.LookRotation(m_Rigidbody.velocity, transform.up), AerodynamicInfluence*Time.deltaTime);
		}

		// Calculate linear and rotational forces.
		Vector3 forces = Vector3.zero;

		forces += transform.forward*m_EnginePower;
		forces += Lift*m_ForwardSpeed*m_ForwardSpeed*m_AeroCorrectionFactor*Mathf.InverseLerp(LiftCutoffSpeed, 0.0f, m_ForwardSpeed)*Vector3.Cross(m_Rigidbody.velocity, transform.right).normalized;
		m_Rigidbody.AddForce(forces);

		forces = Vector3.zero;

		forces += PitchModifier*m_PitchInput*transform.right;
		forces += -RollModifier*m_RollInput*transform.forward;
		forces += YawModifier*m_YawInput*transform.up;
		forces += BankingModifier*m_BankingInfluence*transform.up;
		m_Rigidbody.AddTorque(m_ForwardSpeed*AerodynamicInfluence*forces);
	}
}
