using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebSocketServer.Midddleware
{
    public static class WebSocketMiddlewareExtensions
    {
        //permite utilizar o método no app.UseWebSocketServer();
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketServerConnectionManager>(); //adiciona uma instancia que estará disponivel para uso para a aplicação
            return services;
        }
    }
}