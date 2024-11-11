using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Persistence.Contexts;
using Service.Persistence.Repositories.Interfaces;
using Service.Persistence.Repositories;
using Service.Persistence.UnitOfWork.Interfaces;
using Service.Persistence.UnitOfWork;

namespace Service.Persistence.Utils;

public class RegisterPersistence
{
    public static void Register(WebApplicationBuilder builder)
    {   
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        builder.Services.AddScoped<IFaturaRepository, FaturaRepository>();
        builder.Services.AddScoped<IFaturaItemRepository, FaturaItemRepository>();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
    }
}
