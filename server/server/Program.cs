using System;
using server.Models;
using WebSocketSharp.Server;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer wss = new WebSocketServer("ws://localhost:9000");

            wss.AddWebSocketService<Game>("/Game");
            wss.Start();

            Console.WriteLine("Server --> ws://localhost:9000/Game");

            Console.ReadKey();
            wss.Stop();
        }
    }
}