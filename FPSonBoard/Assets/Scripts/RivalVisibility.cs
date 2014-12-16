using UnityEngine;
using System.Collections;

public class RivalVisibility : MonoBehaviour {

	RivalAI ai;

	// Use this for initialization
	void Start () {
		ai = transform.parent.GetComponent<RivalAI> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Player")
						ai.findPlayer = true;
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Player")
			ai.findPlayer = false;
	}



}
