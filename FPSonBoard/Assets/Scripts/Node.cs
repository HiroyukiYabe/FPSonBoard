using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using WebSocketSharp;
using SocketIOClient.Messages;

using DICT = System.Collections.Generic.Dictionary<string,object>;


public class Node : MonoBehaviour {	

	SocketIOClient.Client socket;
	//WebSocket ws;
	System.Guid guid;
	string _uuid;
	int playerID;

	struct myMessage{
		public int sender;
		public string eventName;
		public Dictionary<string, object> message;
		public myMessage(int _sender, string _event, Dictionary<string, object> _message){
			sender = _sender;
			eventName = _event;
			message = _message;
		}
	}
	Queue<myMessage> msgQueue = new Queue<myMessage>();

	FPSInputController input;
	GameObject player;
	GameObject rival;
	Action<myMessage> ExecuteAction; 

	/*void Init(){
		ws = new WebSocket ("http//127.0.0.1:8888");

		ws.OnMessage+= (sender, e) =>{
			e.Data.GetValue("");
			Console.WriteLine ("Laputa says: " + e.Data);};
	}*/

	void Awake () {
		input = GameObject.FindWithTag("Player").GetComponent<FPSInputController>();
		player = GameObject.FindWithTag("Player");
		rival = GameObject.FindWithTag("Rival");

		//socket = new SocketIOClient.Client("http://127.0.0.1:8888/");
		guid=System.Guid.NewGuid();
		_uuid=guid.ToString();
		this.Connect();
		//ExecuteAction+=ExecuteMsg;
	}
	
	
	void Update () {
		Vector3 vec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Dictionary<string, object> dic = new Dictionary<string,object>();
		dic.Add("x",vec.x);dic.Add("y",vec.y);dic.Add("z",vec.z);
		this.Emit("move",dic);
		//SyncPlayer();
		//while ( 0 < msgQueue.Count){ExecuteMsg(msgQueue.Dequeue());}
		while ( 0 < msgQueue.Count){
			myMessage mm = msgQueue.Dequeue(); 
			if(ExecuteAction != null) ExecuteAction(mm);
		}
	}

	void SyncPlayer(){
		Dictionary<string, object> dic = new Dictionary<string,object>();
		Vector3 playpos = player.transform.position;
		Vector3 playrot = player.transform.rotation.eulerAngles;
		dic.Add("posx",playpos.x);dic.Add("posy",playpos.y);dic.Add("posz",playpos.z);
		dic.Add("rotx",playrot.x);dic.Add("roty",playrot.y);dic.Add("rotz",playrot.z);
		this.Emit("SyncPos",dic);
	}

	void ExecuteMsg(myMessage msg){
		if(msg.eventName == "move" && msg.sender == playerID) OnMove(msg.message);
		if(msg.eventName == "SyncPos") OnSyncPos(msg.sender==playerID,msg.message);
	}

	void OnMove(DICT msg){
		Vector3 vec = new Vector3(float.Parse(msg["x"].ToString()),float.Parse(msg["y"].ToString()),float.Parse(msg["z"].ToString()));
		input.directionVector = vec;
	}
	void OnSyncPos(bool isMine, DICT msg){
		if(!isMine){
			Vector3 pos = new Vector3(float.Parse(msg["posx"].ToString()),float.Parse(msg["posy"].ToString()),float.Parse(msg["posz"].ToString()));
			Vector3 rot = new Vector3(float.Parse(msg["rotx"].ToString()),float.Parse(msg["roty"].ToString()),float.Parse(msg["rotz"].ToString()));
			rival.transform.position = pos;
			rival.transform.rotation = Quaternion.Euler(rot);
		}
	}


	void Connect(){
		socket = new SocketIOClient.Client("http://127.0.0.1:8888/");
		socket.On("connect", (fn) => {
			Debug.Log ("succeed connection - socket");
			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add("GUID", _uuid);
			socket.Emit("connected", args);
		});
		socket.On("playerID", (_playerID) => {
			playerID = _playerID.Json.GetFirstArgAs<int>();
			Debug.Log ("playerID:"+playerID);
		});
		socket.On("message", (data) => {
			string jsonmsg = data.Json.args[0].ToString();
			//Debug.Log("RECV message:"+jsonmsg);
			var msg = MiniJSON.Json.Deserialize(jsonmsg) as Dictionary<string,object>;

			int sen = int.Parse(msg["sender"].ToString());
			string eve = msg["event"].ToString();
			Dictionary<string,object> mes = msg["message"] as Dictionary<string,object>;

			myMessage message = new myMessage(sen,eve,mes);
			msgQueue.Enqueue(message);
		});
		socket.Error += (sender, e) => {
			Debug.LogError ("socket Error: " + e.Message.ToString ());
		};
		socket.Connect();
	}

	public void Emit(string _event, Dictionary<string,object> message){
		Dictionary<string,object> dict = new Dictionary<string, object>();
		dict.Add("sender",playerID);
		dict.Add("event",_event);
		dict.Add("message",message);
		//socket.Send(dict);
		//dict.Add("message",SimpleJson.SimpleJson.SerializeObject(message));
		//Debug.Log(SimpleJson.SimpleJson.SerializeObject(dict));
		//string json = SimpleJson.SimpleJson.SerializeObject(dict);
		string json = MiniJSON.Json.Serialize(dict);
		//Debug.Log("EMIT:"+json);
		socket.Send(json);
	}

	void OnGUI () {
		/*if (!socket.IsConnected) {
			if (GUI.Button (new Rect (20, 20, 150, 30), "start Connection")) {
				Debug.Log ("Try Connection");
				this.Connect();
				//socket.Connect ();
			}
		} else {
			if (GUI.Button (new Rect (20, 20, 150, 30), "Close Connection")) {
				Debug.Log ("Closing");
				socket.Close ();
			}
		}*/
		if (GUI.Button (new Rect (70, 20, 150, 30), "SEND")) {
			ExecuteAction+=ExecuteMsg;
			
			Debug.Log ("SEND");
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("testEvent", "testMsg");
			args.Add("testEvent2", "testMsg2");
			this.Emit("test",args);
		}
	}
	
	void OnApplicationQuit(){
		Debug.Log ("application quit");
		socket.Close ();
	}

}