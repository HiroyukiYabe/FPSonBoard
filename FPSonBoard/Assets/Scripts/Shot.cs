using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

	public GameObject bullet;
	public float speed;
	public float interval;
	float timer; 

	public GameObject rifle;
	private WiiRemote remote;


	// Use this for initialization
	void Start ()  {
		remote =  rifle.GetComponent<WiiRemote>();
	}
	
	// Update is called once per frame
	void  Update () {
		timer += Time.deltaTime;

		if ( remote.buttonBPressed && timer>interval){
			Shoot();
			timer=0f;
		}


//		if (Input.GetButton ("Fire1") && timer>interval) {
//			Shoot();
//			timer=0f;
//		}
	}
	
	void Shoot(){
		GameObject bul = (GameObject)Instantiate (bullet, transform.position, transform.rotation);
		bul.rigidbody.AddForce(transform.forward*speed,ForceMode.VelocityChange);
	}

}
