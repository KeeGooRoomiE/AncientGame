using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightLoadManager : MonoBehaviour {

	/// <summary>
	/// Will be available in the next update (v1.3)
	/// </summary>

	public GameObject playerIcon;
	public GameObject enemyIcon;

	private int playerIconID;
	private int enemyIconID;
	private int gameMode;

	public Texture2D[] availableHeadIcons;

	void Start () {

		gameMode = PlayerPrefs.GetInt ("GAMEMODE");

		playerIconID = 0; //for now as we only have 1 player image

		//if this is a normal game, we are playing against an enemy. otherwise we are playing "BirdHunt" mode.
		if (gameMode == 1)
			enemyIconID = 1;
		else if (gameMode == 2)
			enemyIconID = 2;

		//set correct icons for head slots	
		playerIcon.GetComponent<Renderer>().material.mainTexture = availableHeadIcons[playerIconID];
		enemyIcon.GetComponent<Renderer>().material.mainTexture = availableHeadIcons[enemyIconID];

	}

}
