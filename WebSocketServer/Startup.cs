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
            //request pipeline é formada por uma lista de delegades

            //adiciona o websocket na piline
            app.UseWebSockets();

            //adiciona o delegade
            app.Use(async (context, next) => {
                
                //verifica se a pipeline é um request de web socket
                //se foi feito o pedido de upgrade
                if(context.WebSockets.IsWebSocketRequest)
                {
                    //aceita o pedido de upgrade e cria um objeto web socket
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("WebSocket Connected");
                        
                }
                else
                { //se não for um delegade de pedido de upgrade para uma conexão websocket, prosseguir para o próximo delegade 
                    await next();
                }
            });
        }
    }
}
