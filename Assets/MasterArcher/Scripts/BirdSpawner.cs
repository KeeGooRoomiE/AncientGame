using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour {

	/// <summary>
	/// This class spawn random birds inside the scene.
	/// </summary>

	public GameObject[] birds;			//birds in the menu scene
	private bool canSpawn;

	IEnumerator Start () {
		canSpawn = false;
		yield return new WaitForSeconds (2.0f);
		canSpawn = true;
	}


	void Update () {
		StartCoroutine(birdSpawner ());
	}
		

	IEnumerator birdSpawner() {

		if (!canSpawn)
			yield break;

		int dir = 0;
		if (UnityEngine.Random.value > 0.5f)
			dir = 1;
		else
			dir = -1;

		canSpawn = false;

		GameObject b = Instantiate (birds [UnityEngine.Random.Range (0, 2)], new Vector3 (11 * dir, UnityEngine.Random.Range(3.0f, 5.0f), 0.1f), Quaternion.Euler (0, 180, 0)) as GameObject;
		b.name = "Bird";

		if (Random.value > 0.5f) {
			yield return new WaitForSeconds (0.1f);
			GameObject c = Instantiate (birds [UnityEngine.Random.Range (0, 2)], new Vector3 (11 * dir, UnityEngine.Random.Range(3.0f, 5.0f), 0.1f), Quaternion.Euler (0, 180, 0)) as GameObject;
			c.name = "Bird";
		}

		if (Random.value > 0.85f) {
			yield return new WaitForSeconds (0.1f);
			GameObject d = Instantiate (birds [UnityEngine.Random.Range (0, 2)], new Vector3 (11 * dir, UnityEngine.Random.Range(3.0f, 5.0f), 0.1f), Quaternion.Euler (0, 180, 0)) as GameObject;
			d.name = "Bird";
		}

		yield return new WaitForSeconds (UnityEngine.Random.Range(2.0f, 5.0f));
		canSpawn = true;

	}

}
