using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Portfolio.Common;
using Portfolio.Config;
using Portfolio.Core;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

var connStringBuilder = new NpgsqlConnectionStringBuilder {
  SslMode = SslMode.VerifyFull,
  Host = builder.Configuration["POSTGRES_HOST"],
  Port = builder.Configuration["POSTGRES_PORT"].ParseInt(),
  Database = builder.Configuration["POSTGRES_DBNAME"],
  Username = builder.Configuration["POSTGRES_USERNAME"],
  Password = builder.Configuration["POSTGRES_PASSWORD"]
};

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStringBuilder.ConnectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

// Add services to the container.
var services = builder.Services;

services
    // configure strongly typed settings object
    .Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)))
    .AddDbContext<MasterDbContext>(options => {
      options.UseNpgsql(dataSource).UseSnakeCaseNamingConvention();
    })
    // configure DI for application services
    .AddScoped<UserService>()
    .AddScoped<AuthService>()
    .AddScoped<ImportService>()
    .AddCors()
    .AddRazorPages();

services.AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddControllers();

services.AddHealthChecks();

AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();

app.UseStaticFiles();

app.Use(async (context, next) => {
  await next();
  if (context.Response.StatusCode == 404) {
    context.Request.Path = "/Index";
    await next();
  }
});
app.UseRouting();

// global cors policy
app.UseCors(builder => { builder.AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials(); });

// custom jwt auth middleware
app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.UseExceptionHandler(configure => configure.Run(async context => {
  var error = context.Features.Get<IExceptionHandlerPathFeature>().Error;
  await context.Response.WriteAsJsonAsync(new { message = error.Message, trace = error.StackTrace });
}));

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");

app.MapRazorPages();
app.MapControllers();

await app.RunAsync();
