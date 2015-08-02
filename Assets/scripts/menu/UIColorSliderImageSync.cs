using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIColorSliderImageSync : MonoBehaviour
{
	public Slider TargetR;
	public Slider TargetG;
	public Slider TargetB;

	public bool SyncEveryFrame = false;

	private Image m_Image;

	public void Awake()
	{
		m_Image = this.GetComponent<Image>();

		SyncColor();
	}

	public void Update()
	{
		if(SyncEveryFrame) {
			SyncColor();
		}
	}

	public void SyncColor()
	{
		Color slider_color = m_Image.color;

		slider_color.r = TargetR.normalizedValue;
		slider_color.g = TargetG.normalizedValue;
		slider_color.b = TargetB.normalizedValue;

		m_Image.color = slider_color;
	}
}
