using Common.Logging;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting Ordering API up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using var scope = app.Services.CreateScope();
    var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
    await orderContextSeed.InitializeAsync();
    await orderContextSeed.SeedAsync();

    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
        throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down Ordering API complete");
    Log.CloseAndFlush();
}
