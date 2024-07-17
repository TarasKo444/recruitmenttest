using Microsoft.Extensions.DependencyInjection;
using RecruitmentTest.Application.Services;

namespace RecruitmentTest.Application;

public static class Entry
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<AuthService>();
        services.AddTransient<FintachartsApiService>();
        return services;
    }
}
