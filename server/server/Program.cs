using server.Models;
using WebSocketSharp.Server;

WebSocketServer wss = new("ws://localhost:9000");

wss.AddWebSocketService<Game>("/Game");
wss.Start();

Console.WriteLine("server -> ws://localhost:9000/Game");

Console.ReadKey();
wss.Stop();