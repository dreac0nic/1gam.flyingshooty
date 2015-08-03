using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using UnityEngine.Networking;

public class UINetworkActivitySwitcher : MonoBehaviour
{
	public List<GameObject> EnableOnActivity = new List<GameObject>();
	public List<GameObject> DisableOnActivity = new List<GameObject>();

	private bool m_Active = false;

	public void Update()
	{
		if(NetworkManager.singleton != null && NetworkManager.singleton.isNetworkActive != m_Active && NetworkManager.singleton.client != null && NetworkManager.singleton.client.isConnected) {
			foreach(GameObject item in EnableOnActivity) {
				item.SetActive(NetworkManager.singleton.isNetworkActive);
			}

			foreach(GameObject item in DisableOnActivity) {
				item.SetActive(!NetworkManager.singleton.isNetworkActive);
			}

			m_Active = NetworkManager.singleton.isNetworkActive;
		}
	}
}
