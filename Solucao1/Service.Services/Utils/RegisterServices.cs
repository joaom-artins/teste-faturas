using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Service.Commons.Notifications;
using Service.Commons.Notifications.Interfaces;
using Service.Services.Interfaces;
using Service.Services.Services;

namespace Service.Services.Utils;

public class RegisterServices
{
    public static void Register(WebApplicationBuilder builder)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        builder.Services.AddSingleton(jsonSerializerOptions);

        builder.Services.AddScoped<INotificationContext, NotificationContext>();
        builder.Services.AddScoped<IFaturaService, FaturaService>();
        builder.Services.AddScoped<IFaturaItemService, FaturaItemService>();
        builder.Services.AddScoped<IFaturaManagementService, FaturaManagementService>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
