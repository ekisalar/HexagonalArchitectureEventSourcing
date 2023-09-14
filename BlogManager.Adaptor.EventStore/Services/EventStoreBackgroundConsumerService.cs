using BlogManager.Adaptor.EventStore.Infrastructure;
using BlogManager.Core.Consumer;
using BlogManager.Core.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace BlogManager.Adaptor.EventStore.Services
{
    public class EventStoreBackgroundConsumerService : BackgroundService
    {
        private readonly IEventStoreGenerateReadModelService _eventStoreGenerateReadModelService;
        private readonly IConfiguration           _configuration;
        private readonly IBlogManagerLogger       _logger;

        public EventStoreBackgroundConsumerService(IEventStoreGenerateReadModelService eventStoreGenerateReadModelService,
                                                   IConfiguration           configuration,
                                                   IBlogManagerLogger       logger)
        {
            _eventStoreGenerateReadModelService = eventStoreGenerateReadModelService;
            _configuration           = configuration;
            _logger                  = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var eventStoreConfiguration = _configuration.GetSection("EventStore").Get<EventStoreConfiguration>();

            try
            {
                await _eventStoreGenerateReadModelService.ConnectToPersistentSubscriptionAsync(eventStoreConfiguration.StreamName, eventStoreConfiguration.GroupName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to connect to Event Store: {ex.Message}", ex);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}