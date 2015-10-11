using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(Text))]
public class UIPlayerSpeedometer : MonoBehaviour
{
	public string TargetTag = "Player";
	public bool FindLocalPlayer = false;

	protected Text m_Text;
	protected Transform m_Target;

	public void Awake()
	{
		m_Text = GetComponent<Text>();
		m_Target = findTarget();
	}

	public void Update()
	{
		if(m_Target) {
			Rigidbody target_body = m_Target.GetComponent<Rigidbody>();

			if(m_Text && target_body) {
				float speed = m_Target.transform.InverseTransformDirection(target_body.velocity).z;

				if(speed >= 0.01) {
					m_Text.text = speed.ToString("N");
				} else {
					m_Text.text = (0.0f).ToString("N");
				}
			}
		} else {
			m_Target = findTarget();
		}
	}

	protected Transform findTarget()
	{
		foreach(GameObject possible_target in GameObject.FindGameObjectsWithTag(TargetTag)) {
			NetworkIdentity net_id = possible_target.GetComponent<NetworkIdentity>();

			if(!FindLocalPlayer || (net_id && net_id.isLocalPlayer)) {
				return possible_target.transform;
			}
		}

		return null;
	}
}
