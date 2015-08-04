using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

public class CustomLobbyManager : NetworkLobbyManager
{
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		PlayerLobbyConfiguration lobby = lobbyPlayer.GetComponent<PlayerLobbyConfiguration>();
		PlayerConfiguration game = gamePlayer.GetComponent<PlayerConfiguration>();

		game.PlayerName = lobby.Name;
		game.PlayerColor = lobby.AccentColor;

		return true;
	}
}
