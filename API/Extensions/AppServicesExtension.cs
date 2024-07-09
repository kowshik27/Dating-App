using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class AppServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services,
     IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
