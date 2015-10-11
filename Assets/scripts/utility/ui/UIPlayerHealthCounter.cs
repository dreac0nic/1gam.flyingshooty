using System.Collections;
﻿using UnityEngine;
﻿using UnityEngine.UI;
﻿using UnityEngine.Networking;

[RequireComponent(typeof(Text))]
public class UIPlayerHealthCounter : MonoBehaviour
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
			Living target_health = m_Target.GetComponent<Living>();

			if(m_Text && target_health) {
				m_Text.text = ((int)target_health.Health).ToString();
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
