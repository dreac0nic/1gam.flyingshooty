using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

public class SyncLobbyPlayerToLobbyDelegate : NetworkBehaviour
{
	public GameObject m_PlayerListingEntry = null;

	public override void OnStartClient()
	{
		m_PlayerListingEntry = UILobbyDelegate.AddToPlayerList(this.GetComponent<NetworkLobbyPlayer>());
	}

	public override void OnNetworkDestroy()
	{
		if(m_PlayerListingEntry) {
			Destroy(m_PlayerListingEntry);
		}
	}

	public override void OnStartLocalPlayer()
	{
		UILobbyDelegate.LocalLobbyPlayer = this.GetComponent<NetworkLobbyPlayer>();
	}
}
