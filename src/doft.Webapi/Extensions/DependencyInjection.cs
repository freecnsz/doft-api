using doft.Application.Commands.Account;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Application.Interfaces.ServiceInterfaces;
using doft.Application.Settings;
using doft.Infrastructure;
using doft.Infrastructure.Repositories;
using doft.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using doft.Application.Validators.Account;
using FluentValidation;
using doft.Infrastructure.Repositories.Auth;


namespace doft.Webapi.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddTransient<IJwtService, JwtService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            return services;
        }
    }
}