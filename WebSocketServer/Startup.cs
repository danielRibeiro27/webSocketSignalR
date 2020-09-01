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
using WebSocketServer.Midddleware;

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
            app.UseWebSocketServer();

            //último delegate na pipeline
            //não é necessário o objeto next pois é o último delegate
            app.Run(async context => {
                Console.WriteLine("Hello from the 3rd request delegate.");

                //escreve a string na resposta
                await context.Response.WriteAsync("Hello from the 3rd request delegate.");
            });
        }

    }
}
