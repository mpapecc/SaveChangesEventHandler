using SaveChangesEventHandlers.Example.Models;
using SaveChangesEventHandlers.Example.Repositories.Abstraction;
using SaveChangesEventHandlers.Example.Repositories.Implementation;
using SaveChangesEventHandlers.Example.Services.Abstraction;
using SaveChangesEventHandlers.Example.Services.Implementation;
using SaveChangesEventHandlers.Example.Utils.Mappings.Abstraction;
using SaveChangesEventHandlers.Example.Utils.Mappings.Implementation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core;
using SaveChangesEventHandlers.Example;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddSaveChangesInfrastructure();

builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<IValidator<Contact>, ContactValidator>();
builder.Services.AddSingleton<ICustomMap, CustomMap>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllers();

app.Run();
