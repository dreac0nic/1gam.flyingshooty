using System.Collections;
﻿using UnityEngine;
﻿using UnityEngine.Networking;

public class ScoreHandler : NetworkBehaviour
{
	public uint Frags {
		get { return m_Frags; }
	}

	public uint Deaths {
		get { return m_Deaths; }
	}

	public int Score {
		get { return m_Score; }
	}

	[SyncVar, SerializeField] private uint m_Frags = 0;
	[SyncVar, SerializeField] private uint m_Deaths = 0;
	[SyncVar, SerializeField] private int m_Score = 0;

	public virtual void AddFrags(uint value = 1)
	{
		if(isServer) {
			m_Frags += value;
		}
	}

	public virtual void AddDeaths(uint value = 1)
	{
		if(isServer) {
			m_Deaths += value;
		}
	}

	public virtual void AddScore(int value)
	{
		if(isServer) {
			m_Score += value;
		}
	}
}
