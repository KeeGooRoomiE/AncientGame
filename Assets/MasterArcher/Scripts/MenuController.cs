using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	/// <summary>
	/// Main Menu Controller.
	/// This class handles all touch events on menu buttons.
	/// </summary>

	/// <summary>
	/// Available game modes
	/// 1 = normal player vs computer
	/// 2 = bird hunting
	/// ...
	/// </summary>
	public static int gameMode = 1;

	private float buttonAnimationSpeed = 12;	//speed on animation effect when tapped on button
	private bool canTap = true;					//flag to prevent double tap
	public AudioClip tapSfx;					//tap sound for buttons click

	public GameObject coinLabel;				//coin text on menu scene

	void Awake (){

		//debug
		//PlayerPrefs.DeleteAll();

		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.002f;

		//show avilable player coins in menu scene
		coinLabel.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("PlayerCoins", 0).ToString();
	}


	//*****************************************************************************
	// FSM
	//*****************************************************************************
	void Update (){	
		
		if(canTap) {
			StartCoroutine(tapManager());
		}
			
	}

	
	//*****************************************************************************
	// This function monitors player touches on menu buttons.
	// detects both touch and clicks and can be used with editor, handheld device and 
	// every other platforms at once.
	//*****************************************************************************
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){
		
		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;
		
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {
				
			//Game Modes
			case "Button-01":
				gameMode = 1;
				PlayerPrefs.SetInt ("GAMEMODE", gameMode);
				playSfx(tapSfx);							//play touch sound
				StartCoroutine(animateButton(objectHit));	//touch animation effect
				yield return new WaitForSeconds(1.0f);		//Wait for the animation to end
				SceneManager.LoadScene("Game");				//Load the next scene
				break;

			case "Button-02":
				gameMode = 2;
				PlayerPrefs.SetInt ("GAMEMODE", gameMode);
				playSfx(tapSfx);							//play touch sound
				StartCoroutine(animateButton(objectHit));	//touch animation effect
				yield return new WaitForSeconds(1.0f);		//Wait for the animation to end
				SceneManager.LoadScene("BirdHunt");				//Load the next scene
				break;

			}	
		}
	}

	
	//*****************************************************************************
	// This function animates a button by modifying it's scales on x-y plane.
	// can be used on any element to simulate the tap effect.
	//*****************************************************************************
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;		//initial scale	
		Vector3 destinationScale = startingScale * 1.1f;		//target scale
		
		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = new Vector3( Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
			                                        Mathf.SmoothStep(startingScale.y, destinationScale.y, t),
			                                        _btn.transform.localScale.z);
			yield return 0;
		}
		
		//Scale down
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * buttonAnimationSpeed;
				_btn.transform.localScale = new Vector3( Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
				                                        Mathf.SmoothStep(destinationScale.y, startingScale.y, r),
				                                        _btn.transform.localScale.z);
				yield return 0;
			}
		}
		
		if(r >= 1)
			canTap = true;
	}


	//*****************************************************************************
	// Play sound clips
	//*****************************************************************************
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}
	
}