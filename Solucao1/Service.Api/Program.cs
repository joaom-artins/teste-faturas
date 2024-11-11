using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Service.Business.Utils;
using Service.Commons.Middlewares;
using Service.Persistence.Contexts;
using Service.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Service.Services.Utils;   
using System.Text.Json.Serialization;
using Service.Api.Utils;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using Service.Entities.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
.AddControllers(options =>
{
    options.ModelValidatorProviders.Clear();
    options.Filters.Add(new ConsumesAttribute("application/json"));
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add<NotificationFilter>();
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
builder.Services.AddValidatorsFromAssemblyContaining<FaturaItemCreateRequest>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

RegisterBusiness.Register(builder);
RegisterServices.Register(builder);
RegisterPersistence.Register(builder);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

app.UseMiddleware(typeof(JsonNormalizationMiddleware));
app.UseMiddleware(typeof(ExceptionMiddleware));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocExpansion(DocExpansion.None));
}

app.UseCors();

app.UseHttpsRedirection();

app.MapControllers();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
    dbContext!.Database.Migrate();
}

app.Run();
