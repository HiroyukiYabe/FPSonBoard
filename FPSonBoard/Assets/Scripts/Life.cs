using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Life : MonoBehaviour {

	public int life;
	public int damage;

	public GameObject UIText;
	Text _text;

	// Use this for initialization
	void Start () {
		_text = UIText.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		_text.text = gameObject.name + ":" + life;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Bullet" && life>0)
						TakeDamage ();
	}

	void TakeDamage(){
		life -= damage;
		if(life<=0)
			Debug.Log("FINISH!!");
	}

}
