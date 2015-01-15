using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	Transform player;
	public float destroyDistance = 80f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player").transform;
		Destroy (this.gameObject,4f);
	}
	
	// Update is called once per frame
	void Update () {
		//if ((player.position - transform.position).sqrMagnitude > destroyDistance * destroyDistance)
		//				DestroyImmediate (this.gameObject);
	}

	void OnTriggerEnter(Collider col){
		Debug.Log ("Bullet: OnTriggerEnter");
		this.SendMessage ("Explode");
		renderer.enabled = false;
		//this.enabled = false;
		Destroy (this);
	}

	void OnCollisionEnter(Collision col){
		Debug.Log ("Bullet: OnCollisionEnter");
		this.SendMessage ("Explode");
		renderer.enabled = false;
		//this.enabled = false;
		Destroy (this);
	}
}
