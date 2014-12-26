using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
//using SocketIOClient.Messages;

using DICT = System.Collections.Generic.Dictionary<string,object>;

namespace MyWebsocket{
	public class WebsocketClient {
		
		WebSocket ws;
		System.Guid guid;
		string _uuid;
		static int playerID;//Node Serverから返される一意の番号. 現在の実装では切断して再接続すると番号が変わる
		
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
		public WebsocketClient(string url){
			guid=System.Guid.NewGuid();
			_uuid=guid.ToString();
			connected=false;
			ws = new WebSocket(url);
			ws.OnOpen+=(sender, e) => {
				Debug.Log ("open");
			};
			ws.OnClose+=(sender, e) => {
				Debug.Log ("close");
			};
			ws.OnMessage+=(sender, e) => {
				Debug.Log ("message:"+e.Data);
			};
			ws.OnError+=(sender, e) => {
				Debug.LogWarning("error:"+e.Message);
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
			ws.Connect();
		}
		public void Close(){
			ws.Close();
			//socket.Dispose();
		}
		public bool isConnected{get{return ws.IsAlive ;}}
		
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
			ws.Send("my message");
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

