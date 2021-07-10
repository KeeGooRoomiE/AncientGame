using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour {

	/// <summary>
	/// We dont need an enemy in the following game mode IDs:
	/// 2, ...
	/// Other game mode IDs require an enemy
	/// </summary>

	public static bool isEnemyRequired() {
		int currentGameMode = PlayerPrefs.GetInt ("GAMEMODE");
		if (currentGameMode == 2)
			return false;
		else
			return true;
	}

}
