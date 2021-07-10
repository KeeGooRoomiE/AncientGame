using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedGround : MonoBehaviour {

	/// <summary>
	/// This class creates a lot of small (tall/wide) cube objects and by contecting them together, 
	/// we can build a nice curved ground.
	/// </summary>

	public GameObject bgHolder;		//all instantiated cubes will be a child of this holder
	public GameObject tile;			//tile prefab

	// Do not edit //
	float segmentSize = 1.0f;
	int numSegments = 180;
	Vector3 nextPosition = new Vector3 (-80, -0.6f, 0.5f);
	// Do not edit //

	void Start() {
		createCurvedBg ();
	}

	void createCurvedBg() {

		for (int i = 0; i < numSegments; ++i) {
			Vector3 p1 = nextPosition;
			Vector3 p2 = p1;
			p2.x += segmentSize;

			float r = 1.15f;
			float s = (Mathf.PI * 2f) * 0.03f;
			p1.y += Mathf.Sin (p1.x * s) * r;
			p2.y += Mathf.Sin (p2.x * s) * r;

			float angle = Mathf.Atan2 (p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;
			Vector3 scale = new Vector3 ();
			scale.x = Vector3.Distance (p1, p2) * 1.4f;
			scale.y = 3.8f;    
			scale.z = 1f;

			GameObject t = Instantiate (tile);
			t.name = "curvedBg";
			t.transform.parent = bgHolder.transform;
			t.transform.localScale = scale;
			t.transform.localPosition = p1;
			t.transform.localRotation = Quaternion.identity;
			t.transform.Rotate (0f, 0f, angle);

			nextPosition.x += segmentSize;
		}

	}
}
