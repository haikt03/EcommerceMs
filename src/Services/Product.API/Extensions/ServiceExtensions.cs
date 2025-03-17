using Contracts.Domains.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;

namespace Product.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.ConfigureProductDbContext(configuration);
            services.AddInfrastructureServices();
            services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

            return services;
        }

        private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            var builder = new MySqlConnectionStringBuilder(connectionString);

            services.AddDbContext<ProductContext>
            (
                m => m.UseMySql
                (
                    builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString),
                    e =>
                    {
                        e.MigrationsAssembly("Product.API");
                        e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                    }
                )
            );

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBase<,,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
