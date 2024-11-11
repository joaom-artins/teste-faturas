using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Service.Business.Business;
using Service.Business.Interfaces;

namespace Service.Business.Utils;

public class RegisterBusiness
{
    public static void Register(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IFaturaBusiness, FaturaBusiness>();
        builder.Services.AddScoped<IFaturaItemBusiness, FaturaItemBusiness>();
    }
}