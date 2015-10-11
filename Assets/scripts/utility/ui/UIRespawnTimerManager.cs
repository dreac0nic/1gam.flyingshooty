using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
﻿using UnityEngine.UI;
﻿using UnityEngine.Networking;

public class UIRespawnTimerManager : MonoBehaviour
{
	public string TargetTag = "Player";
	public bool FindLocalPlayer = false;

	public Text Timer;
	public Slider LeftSlider;
	public Slider RightSlider;

	protected Living m_Target;
	protected List<Graphic> m_Graphics;
	protected bool m_IsEnabled = false;

	public void Awake()
	{
		m_Target = findTarget();
		m_Graphics = new List<Graphic>(GetComponentsInChildren<Graphic>());
		setGraphicsEnable(false);
	}

	public void Update()
	{
		if(m_Target) {
			if(!m_Target.IsAlive) {
				if(!m_IsEnabled) {
					setGraphicsEnable(true);
				}

				if(Timer) {
					float time_left = Mathf.Clamp(m_Target.RemainingRespawnCooldown, 0.0f, m_Target.RespawnTime);

					if(time_left > 0.0f) {
						Timer.text = time_left.ToString("F2");
					} else {
						Timer.text = "READY";
					}
				}

				if(LeftSlider) {
					LeftSlider.maxValue = m_Target.RespawnTime;
					LeftSlider.value = m_Target.RemainingRespawnCooldown;
				}

				if(RightSlider) {
					RightSlider.maxValue = m_Target.RespawnTime;
					RightSlider.value = m_Target.RemainingRespawnCooldown;
				}
			} else {
				setGraphicsEnable(false);
			}
		} else {
			m_Target = findTarget();
		}
	}

	protected Living findTarget()
	{
		foreach(GameObject possible_target in GameObject.FindGameObjectsWithTag(TargetTag)) {
			NetworkIdentity net_id = possible_target.GetComponent<NetworkIdentity>();

			if(!FindLocalPlayer || (net_id && net_id.isLocalPlayer)) {
				return possible_target.GetComponent<Living>();
			}
		}

		return null;
	}

	protected void setGraphicsEnable(bool value)
	{
		foreach(Graphic visual in m_Graphics) {
			visual.enabled = value;
		}

		m_IsEnabled = value;
	}
}
