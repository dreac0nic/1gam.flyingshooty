using System.Collections;
﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIApplicationManager : MonoBehaviour
{
	public Text IPAddressText;

	public void StartHost()
	{
		NetworkManager.singleton.StartHost();
	}

	public void JoinGame()
	{
		if(IPAddressText && !System.String.IsNullOrEmpty(IPAddressText.text.Trim())) {
			NetworkManager.singleton.networkAddress = IPAddressText.text.Trim();
		}

		NetworkManager.singleton.StartClient();
	}

	public void ExitApplication()
	{
		Application.Quit();
	}
}
