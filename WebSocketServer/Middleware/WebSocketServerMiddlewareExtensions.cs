using Microsoft.AspNetCore.Builder;

namespace WebSocketServer.Midddleware
{
    public static class WebSocketMiddlewareExtensions
    {
        //permite utilizar o método no app.UseWebSocketServer();
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder){
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }
    }
}