using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	public GameObject explosionTemplate;
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//Direction of the ray
		Vector3 fwd = transform.TransformDirection (Vector3.forward);

		if (Physics.Raycast (transform.position, fwd, out hit, 2.5f) && hit.collider.gameObject.tag == "Enemy") {
			explode();
			//Send message to the npc that got killed
			hit.collider.gameObject.SendMessage("Dead");
			Destroy(gameObject);
		}

	}

	//Instatiate explosion and kill enemy npc
	void explode(){
		GameObject Explosion = (GameObject)Instantiate (explosionTemplate, new Vector3(transform.position.x + 1, transform.position.y, transform.position.z), transform.rotation);
		Explosion.GetComponent<AudioSource>().Play ();
	}
}
