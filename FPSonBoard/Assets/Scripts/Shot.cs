﻿using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

	public GameObject bullet;
	public float speed;
	public float interval;
	float timer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void  Update () {
		timer += Time.deltaTime;

		if ()

		if (Input.GetButton ("Fire1") && timer>interval) {
			GameObject bul = (GameObject)Instantiate (bullet, transform.position, transform.rotation);
			bul.rigidbody.AddForce(transform.forward*speed,ForceMode.VelocityChange);
			timer=0f;
			Debug.Log ("Fire");
		}
			

	}
}
