using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	private int cardsCollected = 0;
	private GameObject gameManager;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision col){
		Debug.Log ("Collision entered");
		//Checks if the player has collided with a memory card
		if (col.gameObject.tag.Contains ("Card")) {
			Debug.Log ("Memory Card Collected");
			Destroy (col.gameObject); //Removes the card from the scene
			cardsCollected += 1;
			audio.Play();
			RefreshGUI (); //Reloads the gui elements
		}

	}

	void RefreshGUI(){
		//This function is when adding new icon to the gui
		GUITexture cardIcon = GameObject.Find ("Card_Icon_" + cardsCollected).guiTexture;
		cardIcon.enabled = true;

		//When the player collects all 5 memory cards complete the level
		if (cardsCollected == 5) {
			if (gameManager != null) {
				gameManager.GetComponent<GameManager>().LevelComplete();		
			}
		}

	}

	public void PlayerCaught(){
		if (gameManager != null) {
			gameManager.GetComponent<GameManager>().RestartLevel();		
		}

	}

}
