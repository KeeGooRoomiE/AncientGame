using UnityEngine;
using System.Collections;

public class SimpleScroller : MonoBehaviour {

	///***********************************************************************
	/// This script will scroll any texture/
	///***********************************************************************

	private float offset;
	private float damper = 0.4f;
	public float coef = 1;

	void LateUpdate (){
		offset += damper * Time.deltaTime * coef;
		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(offset, 0));
	}
}