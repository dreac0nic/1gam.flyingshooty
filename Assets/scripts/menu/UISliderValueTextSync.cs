using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UISliderValueTextSync : MonoBehaviour
{
	public Slider SyncTarget;
	public bool SyncEveryFrame = false;

	private Text m_Text;

	public void Awake()
	{
		m_Text = this.GetComponent<Text>();

		SyncText();
	}

	public void Update()
	{
		if(SyncEveryFrame) {
			SyncText();
		}
	}

	public void SyncText()
	{
		m_Text.text = SyncTarget.value.ToString();
	}
}
