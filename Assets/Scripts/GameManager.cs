using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public int levelNumber;
	private bool levelWon = false, gameComplete = false, gameOver = false;
	bool muted = false;

	// Use this for initialization
	void Start () {
		//Reset gameclock
		Time.timeScale = 1.0f;

		//When the level is started handle randomisation
		setupLevel ();

	}
	
	// Update is called once per frame
	void Update () {
		//Press 'm' to mute all game sounds
		if(Input.GetKeyUp("m")){
			
			if(muted){
				AudioListener.volume = 1;
				muted = false;
			} else {
				AudioListener.volume = 0;
				muted = true;
			}
			
		}

		if (levelWon) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				Application.LoadLevel(levelNumber + 1);		
			} 

			if (Input.GetKeyUp (KeyCode.Backspace)) {
				Application.LoadLevel(0);		
			} 
		}

		if (gameOver) {
			if (Input.GetKeyUp (KeyCode.Space)) {
				Application.LoadLevel(levelNumber);		
			} 
			
			if (Input.GetKeyUp (KeyCode.Backspace)) {
				Application.LoadLevel(0);		
			} 
		}

		if(gameComplete) {
			if (Input.GetKeyUp (KeyCode.Backspace)) {
				Application.LoadLevel(0);		
			} 
		}
	}

	//Handles the setup of each level, eg the random spawns etc...
	void setupLevel(){
		//Randomly chose NPCs
		
		int value1 = Random.Range (1, 7); // gives a value between 1 - 6
		int value2 = Random.Range (1, 7); 
		
		//If the two values are the same
		while (value2 == value1) {
			value2 = Random.Range (1, 7); 
		}
		
		//Disable the two NPCS and their waypoints
		GameObject.Find ("Level" + levelNumber + "_NPC_" + value1).SetActive (false);
		GameObject.Find ("WP" + value1).SetActive (false);
		GameObject.Find ("Level" + levelNumber + "_NPC_" + value2).SetActive (false);
		GameObject.Find ("WP" + value2).SetActive (false);
		
		//Generate a number for which memory card to disable
		value1 = Random.Range (1, 7); // gives a value between 1 - 6
		GameObject.Find ("MemoryCard" + value1).SetActive (false);
		
		//If the level is not level 1, disable traps
		if (levelNumber != 1) {
			value1 = Random.Range (1, 7);
			value2 = Random.Range (1, 7);
			
			//If the two values are the same
			while (value2 == value1) {
				value2 = Random.Range (1, 7); 
			}
			
			GameObject.Find ("C4_Trap_" + value1).SetActive (false);
			GameObject.Find ("C4_Trap_" + value2).SetActive (false);
		}
	}

	public void LevelComplete() {

		//Pause the game
		Time.timeScale = 0;

		if(levelNumber != 4){
			levelWon = true;
			GameObject.Find ("LevelComplete").guiTexture.enabled = true;
		} else {
			gameComplete = true;
			GameObject.Find ("GameComplete").guiTexture.enabled = true;

		}

	}

	public void RestartLevel() {

		//Pause the game
		Time.timeScale = 0;


		gameOver = true;
		GameObject.Find ("GameOver").guiTexture.enabled = true;
	}

}
