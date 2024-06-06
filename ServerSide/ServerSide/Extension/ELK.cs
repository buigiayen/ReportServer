

using Serilog;
public static class RegisterServices
{
    public static IHostBuilder AddSerilog(this IHostBuilder servies, WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.File("logs\\log.log", rollingInterval: RollingInterval.Day , restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
            .MinimumLevel.Verbose()
            .WriteTo.Elasticsearch("http://localhost:9200",
            AppDomain.CurrentDomain.FriendlyName).CreateLogger();
        builder.Host.UseSerilog();
        return servies;
    }
}