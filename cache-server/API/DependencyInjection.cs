using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddDistributedSqlServerCache(options
                =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "Caching";
            });

            return services;
        }

        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
