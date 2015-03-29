using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_Level2_AI : MonoBehaviour {

	//Include proteceted Animator object
	protected Animator animator;

	//Animator bools
	private int canSeePlayer = Animator.StringToHash("canSeePlayer");
	private int patroling = Animator.StringToHash("patroling");
	private int isDead = Animator.StringToHash("isDead");
	
	//States - Remove idle references
	private int patrolingState = Animator.StringToHash("Base Layer.Patroling"); 
	private int followingPlayerState = Animator.StringToHash("Base Layer.FollowingPlayer"); 
	private int followingCrumbState = Animator.StringToHash("Base Layer.FollowingBreadcrumb"); 

	private AnimatorStateInfo state;

	private float fieldOfView = 90;
	public GameObject breadCrumb, player;

	//Handle NPC Death
	private bool dead = false;
	private float deathTimer = 1.0f;

	//Bread crumbing variables
	public List<GameObject> breadCrumbs = new List<GameObject>(); //List to contain all the placed breadcrumbs
	private int breadCrumbCounter = 0, targetBreadcrumb = 0;
	private float speed = 0.2f; //Speed npc travels at
	private Quaternion startingOrientation; //Used to rotate NPC back to starting position when idle

	//Waypoint Variables
	private int waypointCounter;
	public string waypointName;
	public int numberOfWaypoints;
	private GameObject nextWaypoint;
	private bool waypointFound = false;
	
	// Use this for initialization
	void Start () {
		FindClosestWaypoint ();


		//Get animator component
		animator = GetComponent<Animator>();
		startingOrientation = transform.rotation;

		FindPlayer ();

		// Instantiate the breadcrumb game object at the same position we are at
		GameObject BC = (GameObject) Instantiate(breadCrumb, new Vector3(transform.position.x, 0, transform.position.z), transform.rotation);
		breadCrumbs.Add (BC);
		
	}
	
	// Update is called once per frame
	void Update () {
		FindPlayer ();

		state = animator.GetCurrentAnimatorStateInfo (0);

		if(state.nameHash == patrolingState){

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
		else if(state.nameHash == followingPlayerState ){

			//Set the target to move toward at ground level
			//1.8f is head height
			Vector3 target = new Vector3 (player.transform.position.x,0,player.transform.position.z);
			
			transform.LookAt (target);
			transform.Translate (Vector3.forward * (Time.deltaTime * speed));
			
			if (Vector3.Distance (transform.position, breadCrumbs [breadCrumbCounter].transform.position) > 1) {
				GameObject BC = (GameObject)Instantiate (breadCrumb, new Vector3(transform.position.x, 0.0f, transform.position.z), transform.rotation);
				breadCrumbs.Add (BC);
				breadCrumbCounter += 1;
			}
			
		}
		else if(state.nameHash == followingCrumbState){

			if(breadCrumbCounter > 0){
				
				if(Vector3.Distance(transform.position, breadCrumbs[breadCrumbCounter-1].gameObject.transform.position) < 0.5){
					Destroy(breadCrumbs[breadCrumbCounter]);
					breadCrumbs.Remove(breadCrumbs[breadCrumbCounter]);
					breadCrumbCounter -= 1;

					targetBreadcrumb = breadCrumbCounter;

				} else {
					targetBreadcrumb = breadCrumbCounter-1;
				}

				transform.LookAt (breadCrumbs[targetBreadcrumb].transform.position);
				transform.Translate (Vector3.forward * (Time.deltaTime * speed));

			} else {
				//If the NPC is back to the first breadcrumb start patroling again
				if(Vector3.Distance(transform.position, breadCrumbs[0].gameObject.transform.position) < 0.2){
					animator.SetBool (patroling, true);
					FindClosestWaypoint();
				}
			}
		}

		if (dead) {
			if(transform.position.y > -0.3f){
				transform.Translate(Vector3.down * Time.deltaTime);
			}		

			//Destroy gameObject after specified time
			if(deathTimer >= 0 ){
				deathTimer -= Time.deltaTime;
			} else {
				Destroy(gameObject);
			}
		}
	}
	
	//Function finds the player
	void FindPlayer () {

		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;

		if((Vector3.Angle(rayDirection, transform.forward)) < fieldOfView){ // Detect if player is within the field of view

			//Allows me to specify mask I want the raycast to ignore
			LayerMask ignoreMask = ~(1<<8) ;

			if (Physics.Raycast (transform.position, rayDirection, out hit, Mathf.Infinity, ignoreMask) && hit.collider.gameObject.name == player.gameObject.name) {
				animator.SetBool (canSeePlayer, true);

				//Check if the player is withing catching distance of the npc
				if(Vector3.Distance(player.transform.position,transform.position) <= 1.2f){
					player.GetComponent<PlayerScript>().PlayerCaught();
				}

				if(animator.GetBool(patroling)){ //If spoting the player interupts patrol, reposition first breadcrumb
					breadCrumbs[0].transform.position = new Vector3(transform.position.x, 0, transform.position.z);	
				}

				animator.SetBool(patroling, false);
			} else {
				animator.SetBool (canSeePlayer, false);
			}
		}
	}

	void FindClosestWaypoint () {
		
		string name = "";
		float distance = -1;
		RaycastHit hit;
		
		for (int i = 1; i <= numberOfWaypoints; i++) {

			//Look at each waypoint and cast a raycast in that direction

			transform.LookAt(GameObject.Find(waypointName + i).transform.position);
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

	void Dead(){
		animator.SetBool (isDead, true);
		dead = true;
	}

}
