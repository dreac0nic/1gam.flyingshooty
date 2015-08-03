using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UILobbyManager : MonoBehaviour
{
	public static UILobbyManager singleton = null;

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

				if(value) {
					singleton.m_CurrentLocalPlayerConfiguration = value.GetComponent<PlayerLobbyConfiguration>();
				}
			}
		}
	}

	[Header("Player List Settings")]
	public GameObject PlayerListPrefab;
	public RectTransform PlayerListContentAnchor;

	[Header("Configuration Update")]
	public float PlayerConfigUpdateTimeout = 0.5f;
	public Text PlayerNameText;
	public Slider ColorSliderR;
	public Slider ColorSliderG;
	public Slider ColorSliderB;

	private NetworkLobbyPlayer m_CurrentLocalLobbyPlayer = null;
	private PlayerLobbyConfiguration m_CurrentLocalPlayerConfiguration = null;
	private float m_NextNameUpdateTime;
	private float m_NextColorUpdateTime;

	public static GameObject AddToPlayerList(NetworkLobbyPlayer lobbyPlayer)
	{
		if(singleton && singleton.PlayerListPrefab && singleton.PlayerListContentAnchor) {
			GameObject playerListObject = (GameObject)Instantiate(singleton.PlayerListPrefab, Vector3.zero, Quaternion.identity);

			RectTransform child = playerListObject.GetComponent<RectTransform>();
			child.SetParent(singleton.PlayerListContentAnchor, false);

			UIPlayerListingEntry entry = playerListObject.GetComponent<UIPlayerListingEntry>();
			entry.Player = lobbyPlayer;

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

	public void Update()
	{
		if(m_CurrentLocalPlayerConfiguration) {
			if(Time.time >= m_NextNameUpdateTime && PlayerNameText) {
				m_CurrentLocalPlayerConfiguration.SetPlayerName(PlayerNameText.text);
				m_NextNameUpdateTime = System.Single.MaxValue;
			}

			if(Time.time >= m_NextColorUpdateTime && ColorSliderR && ColorSliderG && ColorSliderB) {
				Color new_color = new Color(ColorSliderR.normalizedValue, ColorSliderG.normalizedValue, ColorSliderB.normalizedValue);
				m_CurrentLocalPlayerConfiguration.SetPlayerAccentColor(new_color);
				m_NextColorUpdateTime = System.Single.MaxValue;
			}
		}
	}

	public void StartHost()
	{
		NetworkManager.singleton.StartHost();
	}

	public void JoinGame()
	{
		NetworkManager.singleton.StartClient();
	}

	public void LeaveGame()
	{
		if(NetworkManager.singleton.isNetworkActive) {
			if(NetworkManager.singleton.client != null) {
				if(m_CurrentLocalLobbyPlayer.isServer) {
					NetworkManager.singleton.StopHost();
				} else {
					NetworkManager.singleton.StopClient();
				}
			} else {
				NetworkManager.singleton.StopServer();
			}
		}
	}

	public void SendReady()
	{
		if(m_CurrentLocalLobbyPlayer) {
			m_CurrentLocalLobbyPlayer.SendReadyToBeginMessage();
		}
	}

	public void MarkNameUpdate()
	{
		m_NextNameUpdateTime = Time.time + PlayerConfigUpdateTimeout;
	}

	public void MarkColorUpdate()
	{
		m_NextColorUpdateTime = Time.time + PlayerConfigUpdateTimeout;
	}
}
