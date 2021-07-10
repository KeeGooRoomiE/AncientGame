using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
		
	//***************************************************************************//
	// This class manages pause and unpause states.
	//***************************************************************************//

	public static bool isGamePaused;
	public static bool enableInput;

	private float savedTimeScale;
	public GameObject pausePlane;
	public GameObject uiCam;

	private GameObject AdManagerObject;

	enum Status { PLAY, PAUSE }
	private Status currentStatus = Status.PLAY;


	//*****************************************************************************
	// Init.
	//*****************************************************************************
	void Awake (){		

		isGamePaused = false;
		enableInput = true;
		
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.002f;
		
		if(pausePlane)
	    	pausePlane.SetActive(false); 

		AdManagerObject = GameObject.FindGameObjectWithTag("AdManager");
	}


	//*****************************************************************************
	// FSM
	//*****************************************************************************
	void Update (){
		//touch control
		touchManager();
		
		//optional pause in Editor & Windows (just for debug)
		if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)) {
			//PAUSE THE GAME
			switch (currentStatus) {
	            case Status.PLAY: 
	            	PauseGame(); 
	            	break;
	            case Status.PAUSE: 
	            	UnPauseGame(); 
	            	break;
	            default: 
	            	currentStatus = Status.PLAY;
	            	break;
	        }
		}
		
		//debug restart
		if(Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
	}


	//*****************************************************************************
	// This function monitors player touches on menu buttons.
	// detects both touch and clicks and can be used with editor, handheld device and 
	// every other platforms at once.
	//*****************************************************************************
	void touchManager (){
		if(Input.GetMouseButtonDown(0)) {
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo)) {
				string objectHitName = hitInfo.transform.gameObject.name;
				//print ("objectHitName: " + objectHitName);
				switch(objectHitName) {
					case "PauseBtn":
						switch (currentStatus) {
				            case Status.PLAY: 
				            	PauseGame();
				            	break;
				            case Status.PAUSE: 
				            	UnPauseGame(); 
				            	break;
				            default: 
				            	currentStatus = Status.PLAY;
				            	break;
				        }
						break;
					
					case "ResumeBtn":
						switch (currentStatus) {
				            case Status.PLAY: 
				            	PauseGame();
				            	break;
				            case Status.PAUSE: 
				            	UnPauseGame(); 
				            	break;
				            default: 
				            	currentStatus = Status.PLAY;
				            	break;
				        }
						break;
					
					case "RestartBtn":
						UnPauseGame();
						SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
						break;
						
					case "MenuBtn":
						UnPauseGame ();
						SceneManager.LoadScene ("Menu");
						break;
				}
			}
		}
	}


	void PauseGame (){
		
		print("Game is Paused...");
		enableInput = false;

		//show an Interstitial Ad when the game is paused
		if(AdManagerObject)
			AdManagerObject.GetComponent<AdManager>().showInterstitial();

		isGamePaused = true;
		uiCam.GetComponent<Camera> ().enabled = false;

		savedTimeScale = Time.timeScale;
	    Time.timeScale = 0;
	    AudioListener.volume = 0;
	    if(pausePlane)
	    	pausePlane.SetActive(true);
	    currentStatus = Status.PAUSE;
	}


	void UnPauseGame (){
		
		print("Unpause");
		isGamePaused = false;
		uiCam.GetComponent<Camera> ().enabled = true;

		StartCoroutine(reactiveInput ());

	    Time.timeScale = savedTimeScale;
	    AudioListener.volume = 1.0f;
		if(pausePlane)
	    	pausePlane.SetActive(false);   
	    currentStatus = Status.PLAY;
	}


	IEnumerator reactiveInput() {
		yield return new WaitForSeconds (0.25f);
		enableInput = true;
	}

}