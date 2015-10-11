using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using UnityEngine.Networking;

public class SimpleShooty : NetworkBehaviour
{
	public GameObject ProjectilePrefab;
	public List<Transform> Cannons;
	public float FireDelay = 1.0f;
	public string FireInput = "Fire1";

	private float m_NextFire = 0.0f;
	private Living m_PlayerHealth;

	public void Awake()
	{
		m_PlayerHealth = GetComponent<Living>();
	}

	public void Update()
	{
		if(isLocalPlayer && Input.GetButton(FireInput) && Time.time >= m_NextFire) {
			m_NextFire = Time.time + FireDelay;

			int index = 0;
			Vector3[] pos = new Vector3[Cannons.Count];
			Quaternion[] rot = new Quaternion[Cannons.Count];

			foreach(Transform cannon in Cannons) {
				pos[index] = cannon.position;
				rot[index] = cannon.rotation;

				index += 1;
			}

			CmdFireLasers(pos, rot);
		}
	}

	[Command]
	private void CmdFireLasers(Vector3[] positions, Quaternion[] rotations)
	{
		if(ProjectilePrefab && (!m_PlayerHealth || m_PlayerHealth.IsAlive)) {
			for(int laser = 0; laser < positions.Length; ++laser) {
				GameObject laser_obj = (GameObject)Instantiate(ProjectilePrefab, positions[laser], rotations[laser]);
				LaserController laser_controller = laser_obj.GetComponent<LaserController>();
				laser_controller.OwnerID = this.netId.Value;

				NetworkServer.Spawn(laser_obj);
			}
		}
	}
}
