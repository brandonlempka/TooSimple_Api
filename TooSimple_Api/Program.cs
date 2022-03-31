using TooSimple_Api.Filters;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Managers.Plaid.TokenExchange;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options
    .Filters
    .Add(typeof(ResponseMap)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string dbConnectionString = builder.Configuration.GetConnectionString("TooSimpleMySql");

builder.Services.AddTransient<ITokenExchangeAccessor, TokenExchangeAccessor>();
builder.Services.AddTransient<IPlaidAccountUpdateAccessor, PlaidAccountUpdateAccessor>();
builder.Services.AddTransient<IAccountAccessor, AccountAccessor>();
builder.Services.AddTransient<IGoalAccessor, GoalAccessor>();
builder.Services.AddTransient<ILoggingAccessor, LoggingAccessor>();

builder.Services.AddTransient<ITokenExchangeManager, TokenExchangeManager>();
builder.Services.AddTransient<IAccountUpdateManager, AccountUpdateManager>();
builder.Services.AddTransient<IBudgetingManager, BudgetingManager>();

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
