using Microsoft.Extensions.DependencyInjection;
using Stock.Business.Implementation;
using Stock.Business.Interfaces;

namespace Stock.Business
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }
    }
}
