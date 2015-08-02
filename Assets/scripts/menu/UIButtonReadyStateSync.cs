using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class UIButtonReadyStateSync : MonoBehaviour
{
	[System.NonSerialized]
	public NetworkLobbyPlayer CurrentLocalPlayer;
	public bool SyncEveryFrame;

	public string ReadyText = "READY";
	public string NotReadyText = "NOT READY";
	public Color ReadyColor = Color.green;
	public Color NotReadyColor = Color.red;

	private Image m_Image;
	private Text m_Text;

	public void Awake()
	{
		m_Image = this.GetComponent<Image>();
		m_Text = this.GetComponentInChildren<Text>();
	}

	public void Update()
	{
		CurrentLocalPlayer = UILobbyDelegate.LocalLobbyPlayer;

		if(SyncEveryFrame) {
			SyncReadyState();
		}
	}

	public void SyncReadyState()
	{
		bool ready = CurrentLocalPlayer != null && CurrentLocalPlayer.readyToBegin;

		if(m_Image) {
			syncColor(ready);
		}

		if(m_Text) {
			syncText(ready);
		}
	}

	private void syncColor(bool ready)
	{
		if(ready) {
			m_Image.color = ReadyColor;
		} else {
			m_Image.color = NotReadyColor;
		}
	}

	private void syncText(bool ready)
	{
		if(ready) {
			m_Text.text = ReadyText;
		} else {
			m_Text.text = NotReadyText;
		}
	}
}
