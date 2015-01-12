using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MySocketIO;


public class Life : MonoBehaviour {

	public int life;
	public int damage;
	public float invisibleTime;
	float timer;

	//public GameObject UIText;
	//Text _text;

	// Use this for initialization
	void Start () {
		//_text = UIText.GetComponent<Text> ();
		//timer=invisibleTime;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		//_text.text = gameObject.name + ":" + life;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Bullet" && life>0 && timer>invisibleTime)
			TakeDamage ();
	}

	void TakeDamage(){
		timer = 0f;
		life -= damage;
		Dictionary<string, object> dic = new Dictionary<string,object>();
		dic.Add("life",life);
		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.Emit("Damaged",dic);
		if(life<=0){
			life = 0;
			GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.Emit("Die");
			Debug.Log("FINISH!!");
		}
	}

}
