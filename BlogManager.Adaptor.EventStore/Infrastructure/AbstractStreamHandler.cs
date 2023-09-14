using System.Text;
using System.Text.Json;
using EventStore.ClientAPI;
using MediatR;

namespace BlogManager.Adaptor.EventStore.Infrastructure
{
    public abstract class AbstractStreamHandler
    {
        public readonly LinkedList<INotification> Events = new();

        private string _streamName { get; }

        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStreamHandler(string streamName, IEventStoreConnection eventStoreConnection)
        {
            _streamName           = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        protected async Task SaveAsync()
        {
            var newEvents = Events.ToList().Select(x => new EventData(Guid.NewGuid(),
                                                                      x.GetType().Name,
                                                                      true,
                                                                      Encoding.UTF8.GetBytes(JsonSerializer.Serialize(x, inputType: x.GetType())),
                                                                      Encoding.UTF8.GetBytes(x.GetType().FullName))).ToList();

            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvents);

            Events.Clear();
        }
    }
}