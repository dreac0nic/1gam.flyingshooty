using System.Collections;
﻿using UnityEngine;
﻿using UnityEngine.UI;

public class UIScoreboardEntryManager : MonoBehaviour
{
	public ScoreHandler Player;
	public Text PlayerNameText;
	public Text FragsText;
	public Text DeathsText;
	public Text ScoreText;

	public void Update()
	{
		updateScores();
	}

	protected void updateScores()
	{
		if(Player) {
			PlayerConfiguration player_info = Player.GetComponent<PlayerConfiguration>();

			if(PlayerNameText && player_info) {
				PlayerNameText.text = player_info.PlayerName;
			}

			if(FragsText) {
				FragsText.text = Player.Frags.ToString();
			}

			if(DeathsText) {
				DeathsText.text = Player.Deaths.ToString();
			}

			if(ScoreText) {
				ScoreText.text = Player.Score.ToString();
			}
		}
	}
}
