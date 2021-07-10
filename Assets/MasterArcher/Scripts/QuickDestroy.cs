using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDestroy : MonoBehaviour {

	public float destroyDelay = 1.5f;

	void Start () {
		Destroy (gameObject, destroyDelay);
	}

}
