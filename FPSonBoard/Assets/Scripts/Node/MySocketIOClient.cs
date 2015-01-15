//SocketIOClientを利用して,Socket.IOで立てたNode.jsサーバと通信するためのクラス
//外部でこのクラスを生成し,イベントハンドラ等を設定する


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient.Messages;

using DICT = System.Collections.Generic.Dictionary<string,object>;

namespace MySocketIO{
public class MySocketIOClient {

	SocketIOClient.Client socket;
	System.Guid guid;
	string _uuid;
	static int playerID;//Node Serverから返される一意の番号. 現在の実装では切断して再接続すると番号が変わる
	public int myID {get {return playerID;}}
		public bool isReady;

	//独自に定義したメッセージの容れ物
	public class MyMessage{
		public int sender {get; private set;}
		public string eventName {get; private set;}
		public DICT message {get; private set;}
		public MyMessage(int _sender, string _event, DICT _message){
			sender = _sender;
			eventName = _event;
			message = _message;
		}
		public bool isMine
		{
			get{return sender == playerID;}
		}
	}
	Queue<MyMessage> msgQueue = new Queue<MyMessage>();
	Action<MyMessage> ExecuteMsg; 
	bool connected;

	//socketの初期化
	public MySocketIOClient(string url){
		guid=System.Guid.NewGuid();
		_uuid=guid.ToString();
		connected=false;
		socket = new SocketIOClient.Client(url);
		
		socket.On("connect", (fn) => {
			Debug.Log ("succeed connection - socket");
			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add("GUID", _uuid);
			socket.Emit("connected", args);
		});
		socket.On("playerID", (_playerID) => {
			connected=true;
			playerID = _playerID.Json.GetFirstArgAs<int>();
			Debug.Log ("playerID:"+playerID);
		});
		//イベント名をクラス外部で自由に設定するため、イベント名はメッセージそのものに持たせる
		//Node Serverとは、全てイベント名”my message”としてやり取りする
		socket.On("my message", (data) => {
			string jsonmsg = data.Json.args[0].ToString();
			//Debug.Log("RECV message:"+jsonmsg);
			var msg = MiniJSON.Json.Deserialize(jsonmsg) as DICT;
			
			int sen = int.Parse(msg["sender"].ToString());
			string eve = msg["event"].ToString();
			DICT mes = msg.ContainsKey("message") ? msg["message"] as DICT : null;
			
			//if(sen!=0){//Node.jsに接続直後、ID:0（誰のIDでもない）からのメッセージを受信するバグが発生したため
				MyMessage message = new MyMessage(sen,eve,mes);
				msgQueue.Enqueue(message);
			//}
		});
		socket.Error += (sender, e) => {
				Debug.LogWarning ("socket Error: " + e.Message );//+ ";"+e.Exception.Message);
		};
	}

		public MySocketIOClient(){connected=false;}
		public void SetURL(string url){
			guid=System.Guid.NewGuid();
			_uuid=guid.ToString();
			connected=false;
			socket = new SocketIOClient.Client(url);
			
			socket.On("connect", (fn) => {
				Debug.Log ("succeed connection - socket");
				Dictionary<string, string> args = new Dictionary<string, string>();
				args.Add("GUID", _uuid);
				socket.Emit("connected", args);
			});
			socket.On("playerID", (_playerID) => {
				connected=true;
				playerID = _playerID.Json.GetFirstArgAs<int>();
				Debug.Log ("playerID:"+playerID);
			});
			socket.On("ready", (fn) => {
				Debug.Log ("Ready");
			});
			//イベント名をクラス外部で自由に設定するため、イベント名はメッセージそのものに持たせる
			//Node Serverとは、全てイベント名”my message”としてやり取りする
			socket.On("my message", (data) => {
				string jsonmsg = data.Json.args[0].ToString();
				//Debug.Log("RECV message:"+jsonmsg);
				var msg = MiniJSON.Json.Deserialize(jsonmsg) as DICT;
				
				int sen = int.Parse(msg["sender"].ToString());
				string eve = msg["event"].ToString();
				DICT mes = msg.ContainsKey("message") ? msg["message"] as DICT : null;
				
				//if(sen!=0){//Node.jsに接続直後、ID:0（誰のIDでもない）からのメッセージを受信するバグが発生したため
				MyMessage message = new MyMessage(sen,eve,mes);
				msgQueue.Enqueue(message);
				//}
			});
			socket.Error += (sender, e) => {
				Debug.LogWarning ("socket Error: " + e.Message );//+ ";"+e.Exception.Message);
			};
		}

	//UnityのGameObjectを触れるのはメインスレッドのみなので、受信したメッセージはメインスレッドで取り出す
	public void OnUpdate(){
		while(this.msgQueue.Count>0){
			MyMessage mm = msgQueue.Dequeue();
			if(ExecuteMsg != null) ExecuteMsg(mm);
		}
	}
	 
	//メッセージを処理するメソッドを登録する
	//第1引数が処理したいイベント名、第2引数が処理内容
	public void AddListener(string eventName, Action<MyMessage> act){
		ExecuteMsg += (s) => {if(s.eventName==eventName) act(s);};
	}

	public void Open(){
		socket.Connect();
	}
	public void Close(){
		this.msgQueue.Clear();
		socket.Close();
	}
	public bool isConnected{get{return socket!=null && socket.IsConnected && this.connected;}}

	//メッセージ送信用の拡張関数
	//第1引数のイベント名で第2引数のメッセージ（辞書形式）を送信する
	public void Emit(string _event, DICT message){
		DICT dict = new DICT();
		dict.Add("sender",playerID);
		dict.Add("event",_event);
		dict.Add("message",message);
		string json = MiniJSON.Json.Serialize(dict);
		//イベント名をクラス外部で自由に設定するため、イベント名はメッセージそのものに持たせる
		//Node Serverとは、全てイベント名”my message”としてやり取りする
		socket.Emit("my message",json);
	}
	public void Emit(string _event){
		DICT dict = new DICT();
		dict.Add("sender",playerID);
		dict.Add("event",_event);
		string json = MiniJSON.Json.Serialize(dict);
		//イベント名をクラス外部で自由に設定するため、イベント名はメッセージそのものに持たせる
		//Node Serverとは、全てイベント名”my message”としてやり取りする
		socket.Emit("my message",json);
	}


}


//objectを明示的にキャスト出来なかったため
static class ParseObject{
	public static int ToInt(this object o){
		return int.Parse(o.ToString());
	}
	public static float ToFloat(this object o){
		return float.Parse(o.ToString());
	}
}

}

