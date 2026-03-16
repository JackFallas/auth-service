using AuthService.Domain.Entities;
using AuthService.Domain.Constants;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

using AuthService.Application.Interfaces;
using AuthService.Application.Services;

namespace AuthService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicaionServices (this IServiceCollection services, IConfiguration configuration)
    {
        // INICIALIZANDO LA CONEXION CON LA BASE DE DATOS
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        // INICIALIZANDO LOS SERVICIOS DE EMAIL
        services.AddScoped<IEmailService, EmailService>();

        services.AddHealthChecks();
        
        return services;
    }
}