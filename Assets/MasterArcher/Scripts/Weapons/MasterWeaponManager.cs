using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterWeaponManager : MonoBehaviour {

	/// <summary>
	/// static value holder for different weapon types
	/// </summary>

	static public int arrowDamage = 25;
	static public int axeDamage = 30;

	//the bomb itself has a little damage. But if exploded at the right time, gives more damage.
	static public int bombDamage = 20;				
	static public int bombExplosionDamage = 40;
	static public float bombExplosionRadius = 3;

	static public int grenadeDamage = 35;
	static public int swordDamage = 50;

}
