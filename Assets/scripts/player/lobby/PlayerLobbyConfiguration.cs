using System.Collections;
﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerLobbyConfiguration : NetworkBehaviour
{
	[SyncVar]
	public string Name = "Player";

	[SyncVar]
	public Color AccentColor = Color.red;

	public bool SetPlayerName(string new_name)
	{
		CmdRequestNameChange(new_name);

		return true;
	}

	public bool SetPlayerAccentColor(Color new_color)
	{
		CmdRequestAccentColorChange(new_color);

		return true;
	}

	[Command]
	private void CmdRequestNameChange(string new_name)
	{
		if(!System.String.IsNullOrEmpty(new_name.Trim())) {
			Name = new_name;
		}
	}

	[Command]
	private void CmdRequestAccentColorChange(Color new_color)
	{
		AccentColor = new_color;
	}
}
