using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MySocketIO;


public class GameController : MonoBehaviour {
	
	Life playerLifeScript;
	int playerlife;
	[HideInInspector]
	public int rivallife;
	
	public GameObject playerUIText;
	public GameObject rivalUIText;
	public GameObject finishText;
	Text playertext;
	Text rivaltext;
	Text finishtext;


	void Awake(){
		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.AddListener("Damaged",OnDamaged);
		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.AddListener("Die",OnDie);
	}
	void OnDamaged(MySocketIOClient.MyMessage msg){
		if(!msg.isMine){
			rivallife = msg.message["life"].ToInt();
		}
	}
	void OnDie(MySocketIOClient.MyMessage msg){
		if(msg.isMine) lose();
		else win();
	}

	// Use this for initialization
	void Start () {
		playerLifeScript = GameObject.FindWithTag("Player").GetComponent<Life>();
		playerlife = playerLifeScript.life;
		rivallife = playerlife;
		playertext = playerUIText.GetComponent<Text> ();
		rivaltext = rivalUIText.GetComponent<Text> ();
		finishtext = finishText.GetComponent<Text> ();
		finishtext.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		playerlife = playerLifeScript.life;

		playertext.text = "player:" + playerlife;
		rivaltext.text = "rival:" + rivallife;
	}

//	void Die(){
//		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.Emit("Die");
//	}
	public void lose(){
		finishtext.text = "YOU LOSE...";
	}
	public void win(){
		finishtext.text = "YOU WIN!!";
	}

}
