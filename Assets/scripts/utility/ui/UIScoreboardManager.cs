using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
﻿using UnityEngine.UI;

public class UIScoreboardManager : MonoBehaviour
{
	public GameObject ScoreboardEntryPrefab;

	protected float m_UpdateTime;
	protected List<ScoreHandler> m_Scorers;
	protected bool m_ShowScores = false;

	public void Awake()
	{
		m_Scorers = new List<ScoreHandler>();
		
		setGraphicsMode(false);
	}

	public void Update()
	{
		if(Input.GetButton("Show Scores")) {
			if(!m_ShowScores) {
				m_ShowScores = true;

				setGraphicsMode(true);
			}
		} else if(m_ShowScores) {
			m_ShowScores = false;

			setGraphicsMode(false);
		}

		if(m_ShowScores) {
			if(Time.time >= m_UpdateTime) {
				if(updateScorersArchive()) {
					foreach(Transform child in this.transform) {
						Destroy(child.gameObject);
					}

					foreach(ScoreHandler score in m_Scorers) {
						GameObject obj = (GameObject)Instantiate(ScoreboardEntryPrefab);

						UIScoreboardEntryManager entry = obj.GetComponent<UIScoreboardEntryManager>();
						entry.Player = score;

						obj.transform.SetParent(this.transform, false);
					}

					m_UpdateTime = Time.time + 2.5f;
				}
			}
		}
	}

	protected bool updateScorersArchive()
	{
		List<ScoreHandler> new_scorers = new List<ScoreHandler>();
		HashSet<ScoreHandler> original_set = new HashSet<ScoreHandler>(m_Scorers);
		HashSet<ScoreHandler> new_set;

		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player")) {
			ScoreHandler scorer = obj.GetComponent<ScoreHandler>();

			if(scorer) {
				new_scorers.Add(scorer);
			}
		}

		new_set = new HashSet<ScoreHandler>(new_scorers);

		if(!original_set.SetEquals(new_set)) {
			m_Scorers = new_scorers;

			return true;
		} else {
			return false;
		}
	}

	protected void setGraphicsMode(bool value)
	{
		foreach(Graphic gfx in GetComponentsInChildren<Graphic>()) {
			gfx.enabled = value;
		}
	}
}
