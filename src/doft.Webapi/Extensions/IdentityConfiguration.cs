using System;
using doft.Domain.Entities;
using doft.Domain.Enums;
using doft.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace doft.Webapi.Extensions;

public static class IdentityConfiguration
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {

        services.AddIdentity<AppUser, AppUserRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 3;
            
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        return services;
    }

 
}
