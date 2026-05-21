using DineFlow.API.Hubs;

namespace DineFlow.API.Extensions
{
    public static class SignalRExtensions
    {
        public static IServiceCollection AddSignalRServices(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }

        public static WebApplication MapSignalRHubs(this WebApplication app)
        {
            app.MapHub<KitchenHub>("/hub/kitchen");
            return app;
        }
    }
}