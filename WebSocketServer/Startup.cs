using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets; //NAMESPACE DO WEBSOCKET
using System.Threading;

namespace WebSocketServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //request pipeline é formada por uma lista de delegates

            //adiciona o websocket na pipeline
            //primeiro request delegate
            app.UseWebSockets();

            //adiciona o novo delegate custom
            app.Use(async (context, next) => {
                WriteRequestParam(context);
                
                //verifica se a pipeline é um request de web socket
                //se foi feito o pedido de upgrade
                if(context.WebSockets.IsWebSocketRequest)
                {
                    //aceita o pedido de upgrade e cria um objeto web socket
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("WebSocket Connected");

                    //espera um resultado e um buffer 
                    await ReceiveMessage(webSocket, async (result, buffer) => {

                        if(result.MessageType == WebSocketMessageType.Text){
                            Console.WriteLine("Message Received");
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
                    await next();
                }
            });

            //último delegate na pipeline
            //não é necessário o objeto next pois é o último delegate
            app.Run(async context => {
                Console.WriteLine("Hello from the 3rd request delegate.");

                //escreve a string na resposta
                await context.Response.WriteAsync("Hello from the 3rd request delegate.");
            });
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

        //imprime as propriedades do http context
        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine("Request Method: " + context.Request.Method);
            Console.WriteLine("Request Protocol: " + context.Request.Protocol);

            if(context.Request.Headers != null){
                foreach(var h in context.Request.Headers){
                    Console.WriteLine("--> " + h.Key + " : " + h.Value);
                }
            }
        }
    }
}
