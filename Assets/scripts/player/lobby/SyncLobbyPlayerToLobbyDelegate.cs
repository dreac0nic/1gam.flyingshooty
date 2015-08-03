using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

public class SyncLobbyPlayerToLobbyDelegate : NetworkBehaviour
{
	public GameObject m_PlayerListingEntry = null;

	public override void OnStartClient()
	{
		m_PlayerListingEntry = UILobbyManager.AddToPlayerList(this.GetComponent<NetworkLobbyPlayer>());
	}

	public override void OnNetworkDestroy()
	{
		if(m_PlayerListingEntry) {
			Destroy(m_PlayerListingEntry);
		}
	}

	public override void OnStartLocalPlayer()
	{
		UILobbyManager.LocalLobbyPlayer = this.GetComponent<NetworkLobbyPlayer>();
	}
}
