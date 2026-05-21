using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Host.UseWindowsService();
builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<CountryCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
