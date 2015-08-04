using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerConfiguration : NetworkBehaviour
{
	public Text PlayerNameplateText;

	[SyncVar(hook="OnPlayerNameChange")]
	public string PlayerName;

	[SyncVar(hook="OnPlayerColorChange")]
	public Color PlayerColor;

	public void OnPlayerNameChange(string value)
	{
		if(PlayerNameplateText) {
			PlayerNameplateText.text = value;
		}
	}

	public void OnPlayerColorChange(Color value)
	{
		foreach(Renderer rend in this.GetComponentsInChildren<Renderer>()) {
			foreach(Material mat in rend.materials) {
				mat.color = value;
			}
		}
	}
}
