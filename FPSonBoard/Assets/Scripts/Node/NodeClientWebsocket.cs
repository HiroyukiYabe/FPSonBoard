using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using MySocketIO;
using MyWebsocket;

public class NodeClientWebsocket : MonoBehaviour {
	
	WebsocketClient sock;
	//MySocketIOClient sock;
	FPSInputController input;
	GameObject player;
	GameObject rival;
	
	// Use this for initialization
	void Awake () {
		input = GameObject.FindWithTag("Player").GetComponent<FPSInputController>();
		player = GameObject.FindWithTag("Player");
		rival = GameObject.FindWithTag("Rival");
		
		/*sock = new MySocketIO("http://127.0.0.1:8888/");
		sock.AddListener("Move",OnMove);
		sock.AddListener("SyncPos",OnSyncPos);
		sock.Open();*/
	}
	
	// Update is called once per frame
	void Update () {
		if(sock!=null && sock.isConnected){
			//Move();
			SyncPos();
			sock.OnUpdate();
		}
	}
	
	void Move(){
		Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Dictionary<string, object> dic = new Dictionary<string,object>();
		dic.Add("x",vec.x);dic.Add("y",vec.y);dic.Add("z",vec.z);
		sock.Emit("Move",dic);
	}
	void SyncPos(){
		Dictionary<string, object> dic = new Dictionary<string,object>();
		Vector3 playpos = player.transform.position;
		Vector3 playrot = player.transform.rotation.eulerAngles;
		dic.Add("posx",playpos.x);dic.Add("posy",playpos.y);dic.Add("posz",playpos.z);
		dic.Add("rotx",playrot.x);dic.Add("roty",playrot.y);dic.Add("rotz",playrot.z);
		sock.Emit("SyncPos",dic);
	}
	
	/*void OnMove(MySocketIOClient.MyMessage msg){
		if(msg.isMine){
			Dictionary<string,object> dict = msg.message;
			Vector3 vec = new Vector3(dict["x"].ToFloat(),dict["y"].ToFloat(),dict["z"].ToFloat());
			input.directionVector = vec;
		}
	}
	void OnSyncPos(MySocketIOClient.MyMessage msg){
		if(!msg.isMine){
			Dictionary<string,object> dict = msg.message;
			Vector3 pos = new Vector3(dict["posx"].ToFloat(),dict["posy"].ToFloat(),dict["posz"].ToFloat());
			Vector3 rot = new Vector3(dict["rotx"].ToFloat(),dict["roty"].ToFloat(),dict["rotz"].ToFloat());
			rival.transform.position = pos;
			rival.transform.rotation = Quaternion.Euler(rot);
		}
	}*/
	
	private string text = "http://127.0.0.1:8888/";
	void OnGUI () {
		text = GUILayout.TextField(text);
		if (sock==null || !sock.isConnected) {
			if (GUILayout.Button ("start Connection")) {
				Debug.Log ("Try Connection");
				
				if(sock==null){
					sock = new WebsocketClient("ws://127.0.0.1:8888/");
					Debug.Log("null");
					//sock = new MySocketIOClient(text);
					//sock.AddListener("Move",OnMove);
					//sock.AddListener("SyncPos",OnSyncPos);
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
	
	void OnApplicationQuit(){sock.Close();}
}
