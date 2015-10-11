using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

public class CameraFlightFollow : MonoBehaviour
{
	public enum RotationTrackingModes { PARALLEL }

	[SerializeField] protected Transform m_Target;
	public string TargetTag = "Player";

	public bool CheckForLocalPlayer = false;
	public Vector3 Offset = -5*Vector3.forward;
	public RotationTrackingModes TrackingMode = RotationTrackingModes.PARALLEL;
	public float PositionLerpSpeed = 1.0f;
	public float RotationLerpSpeed = 1.0f;

	[Header("Death Camera Settings")]
	public float PlayerWatchDistance = 1000.0f;
	public float DeathCamRadius = 15.0f;
	public float DeathCamBobHeight = 5.0f;
	public float DeathCamBobSpeed = 1.0f;
	public float DeathCamCircleSpeed = 1.0f;
	public float DeathCamLerp = 1.0f;
	public float DeathCamSlerp = 1.0f;

	public void Update()
	{
		if(!m_Target) {
			foreach(GameObject possible_target in GameObject.FindGameObjectsWithTag(TargetTag)) {
				NetworkIdentity net_id = possible_target.GetComponent<NetworkIdentity>();

				if(!CheckForLocalPlayer) {
					m_Target = possible_target.transform;

					break;
				} else if(net_id && net_id.isLocalPlayer) {
					m_Target = possible_target.transform;

					break;
				}
			}

			if(m_Target && Debug.isDebugBuild) {
				Debug.Log("CameraFlightFollow: Found \"" + m_Target.gameObject.name + "\" as new target!");
			}
		}
	}

	public void FixedUpdate()
	{
		if(m_Target) {
			Living player_life = m_Target.GetComponent<Living>();

			if(!player_life || player_life.IsAlive) {
				this.transform.position = Vector3.Lerp(this.transform.position, m_Target.position + m_Target.TransformDirection(Offset), PositionLerpSpeed*Time.deltaTime);
				this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, m_Target.localRotation, RotationLerpSpeed*Time.deltaTime);
			} else {
				this.transform.position = Vector3.Lerp(this.transform.position, m_Target.position + DeathCamRadius*(Mathf.Sin(DeathCamCircleSpeed*Time.time)*Vector3.forward + Mathf.Cos(DeathCamCircleSpeed*Time.time)*Vector3.right) + DeathCamBobHeight*Mathf.Sin(DeathCamBobSpeed*Time.time)*Vector3.up, DeathCamLerp*Time.deltaTime);
				this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.LookRotation((m_Target.position - this.transform.position).normalized), DeathCamSlerp*Time.deltaTime);
			}
		}
	}
}
