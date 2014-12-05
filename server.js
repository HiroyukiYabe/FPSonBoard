var io = require('socket.io').listen(8888);
io.set('log level', 3);
// ユーザ管理ハッシュ
var userHash = {};
var playerID=0;

// イベントの定義
io.sockets.on("connection", function (socket) {
  console.log("connection");

  // 接続開始カスタムイベント(接続元ユーザを保存し、他ユーザへ通知)
  socket.on("connected", function (data) {
    console.log("Event:connected");
    playerID++;
    var _guid = data.GUID;
    console.log(_guid + "/ID:" + playerID + "が入室しました");
    userHash[socket.id] = _guid;
    socket.emit("playerID",playerID);
    //io.sockets.emit("publish", {value: msg});
  });

  // メッセージ送信カスタムイベント
  socket.on("message", function (data) {
    console.log("Event:message");
    console.log(data);
    msg = JSON.parse(data);
  	//console.log(msg);
    io.sockets.json.emit("message",msg);

    ///io.sockets.json.emit("message", data);
  });

  //全てのメッセージをそのまま全体に返す
  socket.on("my message", function (data) {
    console.log("Event:my message");
    console.log(data);
    msg = JSON.parse(data);
    //console.log(msg);
    io.sockets.json.emit("my message",msg);

    ///io.sockets.json.emit("message", data);
  });

  // 接続終了組み込みイベント(接続元ユーザを削除し、他ユーザへ通知)
  socket.on("disconnect", function () {
    console.log("Event:disconect");
    if (userHash[socket.id]) {
      var msg = userHash[socket.id] + "が退出しました";
      delete userHash[socket.id];
      io.sockets.emit("disconnect", {value: msg});
    }
  });
});

console.log("server running");