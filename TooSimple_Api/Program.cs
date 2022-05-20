using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TooSimple_Api.Filters;
using TooSimple_Api.Middleware;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.FundingSchedules;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Managers.Authorization;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Goals;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Managers.Plaid.TokenExchange;
using TooSimple_Managers.PlaidAccounts;
using TooSimple_Managers.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(builder => builder.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
}));

// Add services to the container.
builder.Services.AddControllers(options => options
    .Filters
    .Add(typeof(ResponseMap)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

string dbConnectionString = builder.Configuration.GetConnectionString("TooSimpleMySql");

builder.Services.AddTransient<ITokenExchangeAccessor, TokenExchangeAccessor>();
builder.Services.AddTransient<IPlaidSyncAccessor, PlaidSyncAccessor>();
builder.Services.AddTransient<IGoalAccessor, GoalAccessor>();
builder.Services.AddTransient<ILoggingAccessor, LoggingAccessor>();
builder.Services.AddTransient<IUserAccountAccessor, UserAccountAccessor>();
builder.Services.AddTransient<IFundingScheduleAccessor, FundingScheduleAccessor>();
builder.Services.AddTransient<IPlaidTransactionAccessor, PlaidTransactionAccessor>();
builder.Services.AddTransient<IPlaidAccountAccessor, PlaidAccountAccessor>();

builder.Services.AddTransient<ITokenExchangeManager, TokenExchangeManager>();
builder.Services.AddTransient<IAccountUpdateManager, AccountUpdateManager>();
builder.Services.AddTransient<IBudgetingManager, BudgetingManager>();
builder.Services.AddTransient<IGoalManager, GoalManager>();
builder.Services.AddTransient<IAuthorizationManager, AuthorizationManager>();
builder.Services.AddTransient<IPlaidTransactionManager, PlaidTransactionManager>();
builder.Services.AddTransient<IPlaidAccountManager, PlaidAccountManager>();

var app = builder.Build();
app.UseCors("ApiCorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<ExceptionLoggerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
