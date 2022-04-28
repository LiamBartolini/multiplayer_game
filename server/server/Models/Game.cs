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
                    Send($"|conn|accepted-{_clientNo}");

                    string segno = e.Data.Split('-')[1];
                    _clientNo++;
                    if (SimboloCorretto(segno))
                    {
                        _clients.Add(new Tuple<WebSocket, string>(Context.WebSocket, segno));
                    }
                    else
                    {
                        string nuovo;
                        if (segno == "cross")
                            nuovo = "circle";
                        else
                            nuovo = "cross";

                        // aggiunge il nuovo segno alla lista
                        _clients.Add(new Tuple<WebSocket, string>(Context.WebSocket, nuovo));

                        // invia il messaggio con il nuovo simbolo al client
                        Send($"|error|changedSymbol-{nuovo}");
                    }
                }
                else
                {
                    Send("|conn|refused");
                }

                // nel caso in cui il client sia da solo invio un messaggio di attesa
                if (_clientNo == 1)
                    Send("|info|wait");

                // appena si collega anche il secondo client invio un messaggio al primo client
                if (_clientNo == 2)
                    _clients[0].Item1.Send("|info|start");
            }

            if (e.Data.StartsWith("|move|"))
            {
                // se il numero di mosse è >= 4 allora chiede al client la composizione del campo
                if (_numeroMosse >= 4)
                    Send("|check|");

                _numeroMosse++;

                // prendo l'indice della lista dell'utente e invio all'altro client il messaggio ricevuto
                int indice = GetIndex();
                
                // client A ->(|move|cross-1) server -> client B(|move|cross-1) 
                _clients[indice == 0 ? 1 : 0].Item1.Send(e.Data);
            }

            // ricezione della composizione del campo
            if (e.Data.StartsWith("|check|"))
            {
                int index = GetIndex();
                int indiceVincitore = WhoWin(e.Data.Split('|')[2], _clients[index].Item2, index);

                // in caso di nessun vincitore il metodo ritorna -1
                if (indiceVincitore != -1)
                {
                    _clients[indiceVincitore].Item1.Send("|won|");
                    _clients[indiceVincitore == 1 ? 0 : 1].Item1.Send("|lost|");
                }
            }

            if (e.Data == "|restart|")
            {
                _numeroMosse = 0;
                int index = GetIndex();
                _clients[index == 0 ? 1 : 0].Item1.Send("|restart|");
            }

            if (e.Data.StartsWith("|quit|"))
            {
                _clientNo--;

                int index = GetIndex();

                // rimuovo il client dalla lista
                _clients.Remove(_clients[index]);

                // nel caso i due client quittassero contemporneamente non invio nessun messaggio
                if (_clients.Count > 0)
                    _clients[0].Item1.Send("|quit|");
            }
        }

        /// <summary>
        /// controlla che il simbolo sia valido
        /// </summary>
        /// <param name="simbolo">Simbolo del client</param>
        /// <returns>un bool, true se è giusto false se è sbagliato</returns>
        private bool SimboloCorretto(string simbolo)
        {
            foreach (Tuple<WebSocket, string> t in _clients)
                if (t.Item2 == simbolo)
                    return false;
            return true;
        }

        /// <summary>
        /// calcol l'indice del client che ha inviato il messaggio al server
        /// </summary>
        /// <returns>l'indice del client</returns>
        private int GetIndex()
        {
            WebSocket ws = Context.WebSocket;
            
            foreach (Tuple<WebSocket, string> t in _clients)
                if (t.Item1 == ws)
                    return _clients.IndexOf(t);

            return -1;
        }

        /// <summary>
        /// controlla chi ha vinto
        /// </summary>
        /// <param name="campo">campo della partita</param>
        /// <param name="segno">segno del client che invia il campo</param>
        /// <param name="indice">indice del client dentro la lista</param>
        /// <returns>indice del client che ha vinto, o -1 in caso di nessuna vittoria</returns>
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
                Sessions.Broadcast("|draw|");

            return -1;
        }

        private bool Parita(string[] campo)
        {
            for (int i = 0; i < campo.Length; i++)
                if (campo[i] == string.Empty) return false;
            return true;
        }
    }
}