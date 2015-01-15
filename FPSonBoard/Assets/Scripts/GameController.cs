using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MySocketIO;


public class GameController : MonoBehaviour {
	
	Life playerLifeScript;
	[SerializeField]
	int playerlife;
	[SerializeField]
	int rivallife;
	
	public GameObject playerUIText;
	public GameObject rivalUIText;
	public GameObject finishText;
	Text playertext;
	Text rivaltext;
	Text finishtext;

	public static bool isplaying;

//	float invisible = 1.0f;
//	float rivaltimer;
//	Color preColor, color;
//	float flickTime;
//	const float FLICK_SPEED = 4.0f;
//	GameObject rivalgraphic;

	void Awake(){
		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.AddListener("Damaged",OnDamaged);
		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.AddListener("Die",OnDie);

		isplaying = true;

//		rivalgraphic = GameObject.FindWithTag("Rival").transform.FindChild("Jasubot-Pro01").;
//		preColor = rivalgraphic.renderer.material.color;
//		color = preColor;
//		flickTime = 0;
	}
	void OnDamaged(MySocketIOClient.MyMessage msg){
		if(!msg.isMine){
			rivallife = msg.message["life"].ToInt();
			//rivaltimer = 0f;
		}
	}
	void OnDie(MySocketIOClient.MyMessage msg){
		isplaying = false;
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
//		rivaltimer += Time.deltaTime;
//		if (rivaltimer<invisible) {
//			flickTime += Time.deltaTime * FLICK_SPEED;
//			//ここで明滅させている　PingPongで値を前後させLerpでなだらかに繋げて色を変化させている
//			color = Color.Lerp (preColor, preColor * 1.5f, Mathf.PingPong (flickTime, 1.0f));
//			rivalgraphic.renderer.material.color = color;
//		}else{
//			flickTime=0f;
//			rivalgraphic.renderer.material.color = preColor;
//		}


		playerlife = playerLifeScript.life;
		playertext.text = "player:" + playerlife;
		rivaltext.text = "rival:" + rivallife;
	}

//	void Die(){
//		GameObject.FindWithTag("GameController").GetComponent<NodeClient>().sock.Emit("Die");
//	}
	public void lose(){
		finishtext.text = "YOU LOSE...";
		Invoke("StartGame",3f);
	}
	public void win(){
		finishtext.text = "YOU WIN!!";
		Invoke("StartGame",3f);
	}

	void StartGame(){
		playerLifeScript.life = 100;
		playerlife = 100;
		rivallife = 100;
		finishtext.text = "Ready...";
		Invoke("StartGame2",1.5f);
	}
	void StartGame2(){
		finishtext.text = "Go!!";
		isplaying = true;
		Invoke("StartGame3",1.0f);
	}
	void StartGame3(){
		finishtext.text = "";
	}

}
