using UnityEngine;
using System.Collections;

public class NPC_Level1_AI : MonoBehaviour {

	//Waypoint Variables
	int waypointCounter;
	public string waypointName;
	public int numberOfWaypoints;
	private float speed = 0.2f;
	GameObject nextWaypoint;
	bool waypointFound = false;

	//Player variables
	private float fieldOfView = 90;
	public GameObject player;

	// Use this for initialization
	void Start () {
		FindClosestWaypoint ();
	}
	
	// Update is called once per frame
	void Update () {
		//Checks if the player is within sight and can be caught
		FindPlayer ();

		if (waypointFound) { 
			transform.LookAt (nextWaypoint.transform.position);
			transform.Translate (Vector3.forward * (Time.deltaTime * speed));
			//When close enough to the way point, look to the next one
			if ((Vector3.Distance (nextWaypoint.transform.position, gameObject.transform.position)) < 0.5) {
				if (waypointCounter != numberOfWaypoints) {
					waypointCounter += 1;
					nextWaypoint = GameObject.Find (waypointName + waypointCounter);
				} else {
					waypointCounter = 1;
					nextWaypoint = GameObject.Find (waypointName + waypointCounter);
				}
			}
		} else {
			FindClosestWaypoint();
		}
	}

	void FindClosestWaypoint () {

		string name = "";
		float distance = -1;
		RaycastHit hit;

		for (int i = 1; i <= numberOfWaypoints; i++) {

			//Look at each waypoint and cast a raycast in that direction
			Vector3 direction = -transform.position + GameObject.Find(waypointName + i).transform.position;

			if (Physics.Raycast (gameObject.transform.position, direction, out hit)) {

				if (hit.collider.gameObject.name.Contains (waypointName)) {

					float currentDistance = Vector3.Distance (hit.transform.position, gameObject.transform.position);
					//Check if the current way point is closer than the last waypoint found
					if (distance == -1 || currentDistance < distance) {
						distance = currentDistance;
						name = hit.collider.gameObject.name;
						waypointCounter = i;

						nextWaypoint = GameObject.Find(name);
						waypointFound = true;
					}
				}
			}
		}
	}

	//Function finds the player
	void FindPlayer () {
		
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;
		//Allows me to specify mask I want the raycast to ignore
		LayerMask ignoreMask = ~(1<<8) ;
		
		if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfView){ // Detect if player is within the field of view
			
			if (Physics.Raycast (transform.position, rayDirection, out hit, Mathf.Infinity, ignoreMask) && hit.collider.gameObject.name == player.gameObject.name) {

				//Check if the player is withing catching distance of the npc
				if(Vector3.Distance(player.transform.position,transform.position) <= 1.2f){
					player.GetComponent<PlayerScript>().PlayerCaught();
				}
			}
		}
	}
}
