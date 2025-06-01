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
using doft.Application.Interfaces;
using doft.Application.Services.Task;

namespace doft.Webapi.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<S3Settings>(configuration.GetSection("S3Settings"));
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IS3Service, S3Service>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<IDetailRepository, DetailRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<INoteRepository, NoteRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IReminderRepository, ReminderRepository>();
            services.AddTransient<IPreferenceRepository, PreferenceRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<ITagLinkRepository, TagLinkRepository>();
            services.AddTransient<IPlannedTaskRepository, PlannedTaskRepository>();
            services.AddTransient<ITaskSchedulingService, TaskSchedulingService>();
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