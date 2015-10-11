using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkFlightController : FlightController
{
	protected NetworkIdentity m_NetID;

	public override void Awake()
	{
		base.Awake();

		m_NetID = this.GetComponent<NetworkIdentity>();
	}

	public override void Update()
	{
		if(m_NetID.isLocalPlayer) {
			updateInputAngles();
		}
	}

	public override void FixedUpdate()
	{
		if(m_NetID.isLocalPlayer) {
			applyForces();

			if(this.transform.InverseTransformDirection(m_Rigidbody.velocity).z <= 0.1f) {
				Debug.Log("DIRTY INSTANT IMPULSE");
				m_Rigidbody.AddForce(100.0f*this.transform.forward, ForceMode.VelocityChange);
			}
		}
	}
}
