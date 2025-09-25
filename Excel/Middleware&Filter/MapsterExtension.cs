using Mapster;
using System.Reflection;

namespace Excel.Middleware_Filter
{
    public static class MapsterExtension
    {
        public static IServiceCollection AddMapsterCustomize(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
