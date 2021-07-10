using UnityEngine;
using System.Collections;

public class TextureScroller : MonoBehaviour {

	///***********************************************************************
	/// This script will scroll the background textures based on the movement 
	/// of main camera to simulate the feel of distance in background layers.
	///***********************************************************************

	[Range(1.0f, 5.0f)]
	public float coef = 1.2f;

	public GameObject cam;
	
	void Update () {
		transform.position = new Vector3 (cam.transform.position.x / coef, transform.position.y,transform.position.z);
	}

}
