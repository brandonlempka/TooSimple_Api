using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Database;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Managers.Plaid.TokenExchange;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string dbConnectionString = builder.Configuration.GetConnectionString("TooSimpleMySql");

builder.Services.AddTransient<ITokenExchangeAccessor, TokenExchangeAccessor>();
builder.Services.AddTransient<IAccountUpdateAccessor, AccountUpdateAccessor>();
builder.Services.AddTransient<IAccountAccessor, AccountAccessor>();

builder.Services.AddTransient<ITokenExchangeManager, TokenExchangeManager>();
builder.Services.AddTransient<IAccountUpdateManager, AccountUpdateManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
