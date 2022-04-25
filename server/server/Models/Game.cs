using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

namespace server.Models
{
    public class Game : WebSocketBehavior
    {
        private static List<Tuple<WebSocket, string>> _clients = new List<Tuple<WebSocket, string>>();
        private static int _clientNo = 0;
        private static int _numeroMosse = 0;

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
                        _clients.Add(new Tuple<WebSocket, string>(Context.WebSocket, segno));
                    }
                    else
                    {
                        string nuovo = segno == "cross" ? "circle" : "cross";
                        _clients.Add(new Tuple<WebSocket, string>(Context.WebSocket, nuovo));
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

            if (e.Data.StartsWith("|move|"))
            {
                if (_numeroMosse >= 4)
                {
                    Send("|check|");
                }

                _numeroMosse++;
                int indice = GetIndex(e.Data);
                _clients[indice == 0 ? 1 : 0].Item1.Send(e.Data);
            }

            if (e.Data.StartsWith("|check|"))
            {
                int index = GetIndex(e.Data);
                int indiceVincitore = WhoWin(e.Data.Split('|')[2], _clients[index].Item2, index);
                if (indiceVincitore != -1)
                {
                    _clients[indiceVincitore].Item1.Send("|won|");
                    _clients[indiceVincitore == 1 ? 0 : 1].Item1.Send("|lost|");
                }
            }

            if (e.Data == "|restart|")
            {
                _numeroMosse = 0;
                int index = GetIndex(e.Data);
                _clients[index == 0 ? 1 : 0].Item1.Send("|restart|");
            }

            if (e.Data.StartsWith("|quit|"))
            {
                _clientNo--;
                
                // rimuovo il client dalla lista
                _clients.Remove(_clients[GetIndex(e.Data)]);

                if (_clients.Count > 0)
                    _clients[0].Item1.Send("|quit|");
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
            
            foreach (Tuple<WebSocket, string> t in _clients)
                if (t.Item1 == ws)
                    return _clients.IndexOf(t);

            return -1;
        }

        private int WhoWin(string campo, string segno, int indice)
        {
            string[] celle = campo.Split(',');
            // righe
            if (celle[0] == segno && celle[1] == segno && celle[2] == segno)
                return indice;
            if (celle[3] == segno && celle[4] == segno && celle[5] == segno)
                return indice;
            if (celle[6] == segno && celle[7] == segno && celle[8] == segno)
                return indice;

            // colonne
            if (celle[0] == segno && celle[3] == segno && celle[6] == segno)
                return indice;
            if (celle[1] == segno && celle[4] == segno && celle[7] == segno)
                return indice;
            if (celle[2] == segno && celle[5] == segno && celle[8] == segno)
                return indice;

            //diagonali
            if (celle[0] == segno && celle[4] == segno && celle[8] == segno)
                return indice;
            if (celle[2] == segno && celle[4] == segno && celle[6] == segno)
                return indice;

            if (Parita(celle))
            {
                Sessions.Broadcast("|draw|");
            }

            return -1;
        }

        private bool Parita (string[] campo)
        {
            for (int i = 0; i < campo.Length; i++)
                if (campo[i] == string.Empty) return false;
            return true;
        }
    }
}