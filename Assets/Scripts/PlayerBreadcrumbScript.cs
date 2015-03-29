using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This script is responsible for dropping the players breadcrumbs
public class PlayerBreadcrumbScript : MonoBehaviour {
	
	public GameObject playerBreadCrumb;
	public List<GameObject> breadCrumbs = new List<GameObject>();
	private int breadCrumbCounter = 0;
	private static float destroyTime = 0.7f ;//Time until the breadcrumb disapear
	private float timer = destroyTime; 
	

	// Use this for initialization
	void Start () {
		// Instantiate the breadcrumb game object at the same position we are at
		GameObject BC = (GameObject) Instantiate(playerBreadCrumb, new Vector3(transform.position.x, 0.2f, transform.position.z), transform.rotation);
		breadCrumbs.Add (BC);
	}
	
	// Update is called once per frame
	void Update () {
		dropBreadcrumb();
		
		timer -= Time.deltaTime;
		if(timer <= 0){
			popBreadcrumb();
			timer = destroyTime;
		}
	}
	
	void dropBreadcrumb () {
		if (Vector3.Distance (transform.position, breadCrumbs [breadCrumbCounter].transform.position) > 1.5) {
			GameObject BC = (GameObject)Instantiate (playerBreadCrumb, new Vector3(transform.position.x, 0.2f, transform.position.z), transform.rotation);
			breadCrumbs.Add (BC);
			breadCrumbCounter += 1;
		}
	}
	
	//Called to delete olded breadcrumb
	void popBreadcrumb(){
		if(breadCrumbs.Count > 1){
			Destroy(breadCrumbs[0]);
			breadCrumbs.Remove(breadCrumbs[0]);
			breadCrumbCounter -= 1;
		}
	}
}
