using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using UnityEngine.Networking;

public class UILobbyDelegate : MonoBehaviour
{
	public static UILobbyDelegate singleton = null;

	public static NetworkLobbyPlayer LocalLobbyPlayer {
		get {
			if(singleton) {
				return singleton.m_CurrentLocalLobbyPlayer;
			} else {
				return null;
			}
		}
		set {
			if(singleton) {
				singleton.m_CurrentLocalLobbyPlayer = value;
			}
		}
	}

	public GameObject PlayerListPrefab;
	public RectTransform PlayerListContentAnchor;

	private NetworkLobbyPlayer m_CurrentLocalLobbyPlayer = null;

	public static GameObject AddToPlayerList(NetworkLobbyPlayer lobbyPlayer)
	{
		if(singleton && singleton.PlayerListPrefab && singleton.PlayerListContentAnchor) {
			GameObject playerListObject = (GameObject)Instantiate(singleton.PlayerListPrefab, Vector3.zero, Quaternion.identity);

			RectTransform child = playerListObject.GetComponent<RectTransform>();
			child.SetParent(singleton.PlayerListContentAnchor, false);

			return playerListObject;
		} else {
			return null;
		}
	}

	public void Awake()
	{
		if(!singleton) {
			singleton = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	public void SetReadyState(bool state)
	{
		if(m_CurrentLocalLobbyPlayer) {
			m_CurrentLocalLobbyPlayer.readyToBegin = state;
		}
	}

	public void ToggleReadyState()
	{
		if(m_CurrentLocalLobbyPlayer) {
			m_CurrentLocalLobbyPlayer.readyToBegin = !m_CurrentLocalLobbyPlayer.readyToBegin;
		}
	}
}
