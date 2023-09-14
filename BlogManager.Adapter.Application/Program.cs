using BlogManager.Adapter.Api;
using BlogManager.Adapter.Logger;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Adaptor.EventStore.Infrastructure;
using BlogManager.Adaptor.EventStore.Services;
using BlogManager.Core.Consumer;
using BlogManager.Core.Handlers.EventHandlers;
using BlogManager.Core.Logger;
using BlogManager.Core.Repositories;
using EventStore.ClientAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventStoreGenerateReadModelService = BlogManager.Adaptor.EventStore.Services.EventStoreGenerateReadModelService;


var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json",                    optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                   .Build();

var migrationServiceProvider = BuildServiceProvider(new ServiceCollection()
                                                       .AddDbContext<BlogDbContext>(options =>
                                                                                        options.UseNpgsql(configuration.GetConnectionString("BlogDb"),
                                                                                                          b => b.MigrationsAssembly(typeof(BlogDbContext).Assembly.FullName))));

//Auto Migrations
using (var scope = migrationServiceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    dbContext.Database.Migrate();
}


IServiceProvider BuildServiceProvider(IServiceCollection services)
{
    services.AddScoped<IAuthorRepository, AuthorRepository>();
    services.AddScoped<IBlogRepository, BlogRepository>();
    services.AddSingleton<IBlogManagerLogger, SerilogAdapter>();
    services.AddDbContext<IBlogDbContext, BlogDbContext>(c => c.UseNpgsql(configuration.GetConnectionString("BlogDb")));
    services.AddSingleton<IEventStoreConnection>(sp =>
    {
        var factory                 = new EventStoreConnectionFactory();
        var eventStoreConfiguration = configuration.GetSection("EventStore").Get<EventStoreConfiguration>();
        return Task.Run(() => factory.CreateConnectionAsync(
                                                            eventStoreConfiguration.ConnectionString,
                                                            eventStoreConfiguration.StreamName,
                                                            eventStoreConfiguration.GroupName)).Result;
    });

    return services.BuildServiceProvider();
}



// Api initialization
var apiAdapter = new ApiAdapter(args, options =>
{
    BuildServiceProvider(options);
    options.AddSingleton<IBlogManagerStreamHandler, BlogManagerStreamHandlerHandler>();
});

var apiTask = apiAdapter.StartAsync();

// Background Service initialization    
var eventConsumer = Host.CreateDefaultBuilder(args)
                        .ConfigureServices(services =>
                         {
                             BuildServiceProvider(services);
                             services.AddScoped<IEventStoreGenerateReadModelService, EventStoreGenerateReadModelService>();
                             services.AddHostedService<EventStoreBackgroundConsumerService>();
                         })
                        .Build();

await eventConsumer.RunAsync();
await Task.WhenAll(apiTask);