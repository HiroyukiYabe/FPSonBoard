using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MySocketIO;

public class NodeClient : MonoBehaviour {
	
	MySocketIOClient sock;
	//FPSInputController input;
	Transform player;
	PlayerController playerCon;
	Transform rival;
	PlayerController rivalCon;
	wiiBoard Wii;
	
	
	// Use this for initialization
	void Awake () {
		//input = GameObject.FindWithTag("Player").GetComponent<FPSInputController>();
		player = GameObject.FindWithTag("Player").transform;
		playerCon = player.GetComponent<PlayerController>();
		rival = GameObject.FindWithTag("Rival").transform;
		rivalCon = rival.GetComponent<PlayerController>();
		Wii = GameObject.Find("Wii").GetComponent<wiiBoard>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));
		if(input == Vector3.zero)
			input = Wii.WiiBoardInput/30f;

		if(sock!=null && sock.isConnected){
			//Move(input);
			SyncPos();
			if(Input.GetButton ("Fire1")) Shoot();
			sock.OnUpdate();
		}
		//else playerCon.moveDir = input;
		playerCon.moveDir = input;
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
		dic.Add("posx",playpos.x);dic.Add("posy",playpos.y);dic.Add("posz",playpos.z);
		dic.Add("rotx",playrot.x);dic.Add("roty",playrot.y);dic.Add("rotz",playrot.z);
		sock.Emit("SyncPos",dic);
	}
	void Shoot(){
		Dictionary<string, object> dic = new Dictionary<string,object>();
		sock.Emit("Shoot",dic);
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
			rival.position = pos;
			rival.rotation = Quaternion.Euler(rot);
		}
	}
	void OnShoot(MySocketIOClient.MyMessage msg){
		if(msg.isMine) playerCon.Shoot();
		else rivalCon.Shoot();
	}

	private string text = "http://127.0.0.1:8888/";
	void OnGUI () {
		text = GUILayout.TextField(text);
		if (sock==null || !sock.isConnected) {
			if (GUILayout.Button ("start Connection")) {
				Debug.Log ("Try Connection");
				if(sock==null){
					sock = new MySocketIOClient(text);
					sock.AddListener("Move",OnMove);
					sock.AddListener("SyncPos",OnSyncPos);
					sock.AddListener("Shoot",OnShoot);
				}
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
