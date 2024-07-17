using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RecruitmentTest.Infrastructure;

public static class Entry
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("RecruitmentTestDatabase");
        });
        services.AddScoped<DataSeeder>();
        return services;
    }
}
