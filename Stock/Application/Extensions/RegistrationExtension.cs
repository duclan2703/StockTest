using Core;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class RegistrationExtension
    {
        public static IServiceCollection AddRegistrationExtension(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var type in assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && !t.IsInterface))
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));
                foreach (var iface in interfaces)
                {
                    services.AddScoped(iface, type);
                }
            }

            foreach (var type in assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract && !t.IsInterface))
            {
                var validatorInterfaces = type.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
                foreach (var validatorInterface in validatorInterfaces)
                {
                    services.AddScoped(validatorInterface, type);
                }
            }
            return services;
        }
    }
}
