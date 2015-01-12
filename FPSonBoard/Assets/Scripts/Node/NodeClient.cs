using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MySocketIO;

public class NodeClient : MonoBehaviour {

	[HideInInspector]
	public MySocketIOClient sock;
	//FPSInputController input;
	Transform player;
	PlayerController playerCon;
	Transform playerRifle;
	Transform rival;
	PlayerController rivalCon;
	Transform rivalRifle;
	wiiBoard WBoard;
	WiiRemote WRemote;
	
	
	// Use this for initialization
	void Awake () {
		sock = new MySocketIOClient(text);

		//input = GameObject.FindWithTag("Player").GetComponent<FPSInputController>();
		player = GameObject.FindWithTag("Player").transform;
		playerCon = player.GetComponent<PlayerController>();
		playerRifle = player.FindChild("Main Camera/Assault Rifle Low Free");
		rival = GameObject.FindWithTag("Rival").transform;
		rivalCon = rival.GetComponent<PlayerController>();
		rivalRifle = rival.FindChild("Main Camera/Assault Rifle Low Free");
		WBoard = GameObject.Find("Wii").GetComponent<wiiBoard>();
		WRemote = GameObject.Find("Wii").GetComponent<WiiRemote>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));
		if(WBoard != null && input == Vector3.zero)
			input = WBoard.WiiBoardInput/30f;

		if(sock!=null && sock.isConnected){
			//Move(input);
			SyncPos();
			if(Input.GetButton ("Fire1") || (WRemote != null && WRemote.buttonBPressed)) Shoot();
			sock.OnUpdate();
		}
		//else playerCon.moveDir = input;
		playerCon.moveDir = input;
		playerCon.rotateFlag = WRemote==null ? 0f : 
			(WRemote.buttonRightPressed ? 1f : (WRemote.buttonLeftPressed ? -1f : 0f) );
	}


	void Move(Vector3 vec){
		Dictionary<string, object> dic = new Dictionary<string,object>();
		dic.Add("x",vec.x);dic.Add("y",vec.y);dic.Add("z",vec.z);
		sock.Emit("Move",dic);
	}
	void SyncPos(){
		Dictionary<string, object> dic = new Dictionary<string,object>();
		Vector3 playpos = player.position;
		Vector3 playrot = player.rotation.eulerAngles;
		Vector3 riflerot = playerRifle.localRotation.eulerAngles;
		dic.Add("posx",playpos.x);dic.Add("posy",playpos.y);dic.Add("posz",playpos.z);
		dic.Add("rotx",playrot.x);dic.Add("roty",playrot.y);dic.Add("rotz",playrot.z);
		dic.Add("riflex",riflerot.x);dic.Add("rifley",riflerot.y);dic.Add("riflez",riflerot.z);
		sock.Emit("SyncPos",dic);
	}
	void Shoot(){
		sock.Emit("Shoot");
	}

	void OnMove(MySocketIOClient.MyMessage msg){
		if(msg.isMine){
			Dictionary<string,object> dict = msg.message;
			Vector3 vec = new Vector3(dict["x"].ToFloat(),dict["y"].ToFloat(),dict["z"].ToFloat());
			playerCon.moveDir = vec;
			//input.directionVector = vec;
		}
	}
	void OnSyncPos(MySocketIOClient.MyMessage msg){
		if(!msg.isMine){
			Dictionary<string,object> dict = msg.message;
			Vector3 pos = new Vector3(dict["posx"].ToFloat(),dict["posy"].ToFloat(),dict["posz"].ToFloat());
			Vector3 rot = new Vector3(dict["rotx"].ToFloat(),dict["roty"].ToFloat(),dict["rotz"].ToFloat());
			Vector3 riflerot = new Vector3(dict["riflex"].ToFloat(),dict["rifley"].ToFloat(),dict["riflez"].ToFloat());
			rival.position = pos;
			rival.rotation = Quaternion.Euler(rot);
			rivalRifle.localRotation = Quaternion.Euler(riflerot);
		}
	}
	void OnShoot(MySocketIOClient.MyMessage msg){
		if(msg.isMine) playerCon.Shoot();
		else rivalCon.Shoot();
	}
	void OnDamaged(MySocketIOClient.MyMessage msg){
		if(!msg.isMine){
			GetComponent<GameController>().rivallife = msg.message["life"].ToInt();
		}
	}
	void OnDie(MySocketIOClient.MyMessage msg){
		if(msg.isMine) GetComponent<GameController>().lose();
		else GetComponent<GameController>().win();
	}

	private string text = "http://157.82.7.206:8888/";
	void OnGUI () {
		text = GUILayout.TextField(text);
		if (sock==null || !sock.isConnected) {
			if (GUILayout.Button ("start Connection")) {
				Debug.Log ("Try Connection");
				//if(sock==null){
				//	sock = new MySocketIOClient(text);
					sock.AddListener("Move",OnMove);
					sock.AddListener("SyncPos",OnSyncPos);
					sock.AddListener("Shoot",OnShoot);
				//	sock.AddListener("Damaged",OnDamaged);
				//	sock.AddListener("Die",OnDie);
				//}
				sock.Open();
			}
		} else {
			if (GUILayout.Button ("Close Connection")) {
				Debug.Log ("Closing");
				sock.Close ();
			}
		}
	}

	void OnApplicationQuit(){if(sock!=null) sock.Close();}
}
