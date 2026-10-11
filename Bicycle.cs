using Microsoft.Extensions.DependencyInjection;
using VyshKat.Services;

namespace VyshKat;

public static class DependencyConfig
{
    public static ServiceProvider Build(IInspectionService? inspectionService = null)
    {
        var services = new ServiceCollection();
        if (inspectionService is null)
        {
            services.AddSingleton<IInspectionService, ServiceCenter>();
        }
        else
        {
            services.AddSingleton<IInspectionService>(inspectionService);
        }

        services.AddSingleton<Fleet>();
        return services.BuildServiceProvider();
    }
}
