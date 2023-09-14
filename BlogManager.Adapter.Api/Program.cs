using System.Text.Json.Serialization;
using BlogManager.Adapter.Api.Controllers;
using BlogManager.Adapter.Api.Utilities;
using BlogManager.Core.Commands.Blog;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace BlogManager.Adapter.Api;

public class ApiAdapter
{
    private WebApplication _app;

    public ApiAdapter(string[] args, Action<IServiceCollection> options)
    {
        var builder = WebApplication.CreateBuilder(args);

        options.Invoke(builder.Services);
        builder.Services.AddControllers()
               .AddApplicationPart(typeof(BlogController).Assembly)
               .AddXmlSerializerFormatters()
               .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
               .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "BlogManager API", Version = "v1"}); });
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateBlogCommand>());

        _app = builder.Build();

        _app.UseSwagger();
        _app.UseSwaggerUI();
        _app.UseHttpsRedirection();
        _app.UseAuthorization();
        _app.UseMiddleware<ErrorHandlerMiddleware>();
        _app.MapControllers();
    }

    public Task StartAsync()
    {
        return _app.RunAsync();
    }
}