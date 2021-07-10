using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trajectoryPointController : MonoBehaviour {

	/// <summary>
	/// This class changes the scale of helper points and destroys them when there is no input by player.
	/// </summary>

	private Vector3 StartingScale;		//initial scale
	private Vector3 targetScale;		//final small scale


	void Start () {

		StartingScale = transform.localScale;
		targetScale = StartingScale / 4;

		StartCoroutine(changeScale ());

		Destroy (gameObject, 1);
	}


	/// <summary>
	/// Decrese the scale of helper points by time
	/// </summary>
	IEnumerator changeScale () {
		float t = 0;
		while(t < 1) {
			t += Time.deltaTime;
			transform.localScale = new Vector3 (Mathf.SmoothStep (StartingScale.x, targetScale.x, t),
												Mathf.SmoothStep (StartingScale.y, targetScale.y, t),
												Mathf.SmoothStep (StartingScale.z, targetScale.z, t));		
			yield return 0;
		}
	}


	void Update() {

		//destroy helpers when there is no input by player
		if (Input.GetMouseButtonUp (0)) {
			Destroy (gameObject);
		}

	}
}
