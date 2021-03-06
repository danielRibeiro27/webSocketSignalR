using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSocketServer.Midddleware 
{
    //classe que irá conter a lógica do middleware
    //dessa forma é possível utilizar algo como app.UseWebSockets(); na pipeline
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next; //injeção de dependencia
        private readonly WebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //verifica se a pipeline é um request de web socket
            //se foi feito o pedido de upgrade
            if(context.WebSockets.IsWebSocketRequest)
            {
                //aceita o pedido de upgrade e cria um objeto web socket
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");

                string ConnID = _manager.AddSocket(webSocket); //adiciona o web socket na lista e retorna seu ID
                await SendConnIDAsync(webSocket, ConnID);

                //espera um resultado e um buffer 
                await ReceiveMessage(webSocket, async (result, buffer) => {

                    if(result.MessageType == WebSocketMessageType.Text){
                        Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}"); //transforma o buffer em uma string
                        return;
                    }
                    else if(result.MessageType == WebSocketMessageType.Close){
                        Console.WriteLine("Recived Close message");
                        return;
                    }
                    
                });
            }
            else
            { 
                //se não for um delegates de pedido de upgrade para uma conexão websocket, prosseguir para o próximo delegates 
                Console.WriteLine("Hello from the 2rd request delegate.");
                await _next(context);
            }
        }

        private async Task SendConnIDAsync (WebSocket socket, string connId)
        {
            //transforma uma string em bytes
            var buffer = Encoding.UTF8.GetBytes("ConnID: " + connId);

            //retorna o dado para o cliente
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None); 
        }
        
        //método que espera uma mensagem e em seguida retorna
        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4]; //buffer que será populado com a mensagem

            while(socket.State == WebSocketState.Open){

                //trigger quando recebe algo do socket
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                handleMessage(result, buffer); //retorna o resultado e o buffer
            }
        }

    }

}