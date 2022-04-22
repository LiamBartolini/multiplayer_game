using WebSocketSharp;
using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace server.Models
{
    public class Game : WebSocketBehavior
    {
        private static List<Tuple<WebSocket, string>> _clients = new();
        private static int _clientNo = 0;

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);

            if (e.Data.StartsWith("|conn|new"))
            {
                if (_clientNo < 2)
                {
                    string segno = e.Data.Split('-')[1];
                    _clientNo++;
                    if (SimboloCorretto(segno))
                    {
                        _clients.Add(new(Context.WebSocket, segno));
                    }
                    else
                    {
                        string nuovo = segno == "cross" ? "circle" : "cross";
                        _clients.Add(new(Context.WebSocket, nuovo));
                        Send($"|error|changedSymbol-{nuovo}");
                    }

                    Send($"|conn|accepted-{_clientNo}");
                }
                else
                {
                    Send("|conn|refused");
                }

                if (_clientNo == 1)
                {
                    Send("|info|wait");
                }

                // appena si collega anche il secondo client invio un messaggio al primo client
                if (_clientNo == 2)
                {
                    _clients[0].Item1.Send("|info|start");
                }
            }

            if (e.Data.StartsWith("|field|"))
            {
                int indice = GetIndex(e.Data);
                _clients[indice == 0 ? 1 : 0].Item1.Send(e.Data);
            }
        }

        private bool SimboloCorretto(string simbolo)
        {
            foreach (Tuple<WebSocket, string> t in _clients)
                if (t.Item2 == simbolo)
                    return false;
            return true;
        }

        private int GetIndex(string data)
        {
            WebSocket ws = Context.WebSocket;
            string segno = data.Split('|')[2].Split('-')[0];

            return _clients.IndexOf(new(ws, segno));
        }
    }
}