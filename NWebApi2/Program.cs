using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

if (OperatingSystem.IsWindows())
{
    builder.Host.UseWindowsService();
}
else
{
    builder.Host.UseSystemd();
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}", "Employee.db")}"));

builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddSignalR();

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<EmployeeConsumer>();

    options.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            builder.Configuration["RabbitMq:Host"],
            //"/",
            h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"]);
                h.Password(builder.Configuration["RabbitMq:Password"]);
            });
        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("NAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors("NAngularApp");
app.MapHub<MessageHub>("/messagehub");

app.UseAuthorization();

app.MapControllers();

app.Run();
