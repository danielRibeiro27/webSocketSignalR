using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketServer.Midddleware
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        //adiciona o socket e gera um id unico, em seguida o retorna
        public string AddSocket(WebSocket socket)
        {
            string ConnID = Guid.NewGuid().ToString(); //cria um id

            _sockets.TryAdd(ConnID, socket);
            Console.WriteLine("Connection Added: " + ConnID);

            return ConnID;
        }
    }
}