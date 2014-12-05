//websocket版　あまり使えなさそう

var WebSocketServer = require('ws').Server
, wss = new WebSocketServer({port: 8888});
wss.on('connection', function(ws) {
ws.on('message', function(message) {
console.log('received: %s', message);
});
ws.send('something');
});