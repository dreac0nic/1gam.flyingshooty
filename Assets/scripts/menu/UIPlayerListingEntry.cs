using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIPlayerListingEntry : MonoBehaviour
{
	public NetworkLobbyPlayer Player {
		get { return m_Player; }
		set {
			m_Player = value;

			if(Player == null) {
				m_PlayerConfig = null;
			} else {
				m_PlayerConfig = m_Player.GetComponent<PlayerLobbyConfiguration>();
			}
		}
	}

	public Text PlayerNameText;
	public Image PlayerAccentColorImage;
	public RectTransform PlayerReadyOverlay;

	private NetworkLobbyPlayer m_Player;
	private PlayerLobbyConfiguration m_PlayerConfig;
	private float m_LerpTargetEnd = System.Single.MinValue;

	public void Update()
	{
		if(m_PlayerConfig) {
			if(PlayerNameText) {
				PlayerNameText.text = m_PlayerConfig.Name;
			}

			if(PlayerAccentColorImage) {
				PlayerAccentColorImage.color = m_PlayerConfig.AccentColor;
			}

			if(m_Player.readyToBegin && m_LerpTargetEnd <= System.Single.MinValue) {
				m_LerpTargetEnd = Time.time + 0.3f;
			}
		} else if(m_Player) {
			m_PlayerConfig = m_Player.GetComponent<PlayerLobbyConfiguration>();
		}

		if(Time.time < m_LerpTargetEnd) {
			PlayerReadyOverlay.localScale = Vector3.Lerp(Vector3.one, new Vector3(14.0f, 1.0f, 1.0f), 1.0f - (m_LerpTargetEnd - Time.time)/0.3f);
		}
	}
}
