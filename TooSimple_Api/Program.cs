using FluentMigrator.Runner;
using TooSimple_Database.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database migrations.
//builder.Services.AddFluentMigratorCore()
//    .ConfigureRunner(runner => runner
//    .AddMySql5()
//    .WithGlobalConnectionString("server=localhost:3306;uid=root;pwd=admin;database=test")
//    .ScanIn(typeof(FirstMigration).Assembly).For.Migrations())
//    .AddLogging(logger => logger.AddFluentMigratorConsole())
//    .BuildServiceProvider(false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Services
//    .CreateScope()
//    .ServiceProvider
//    .GetRequiredService<IMigrationRunner>()
//    .MigrateUp();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
