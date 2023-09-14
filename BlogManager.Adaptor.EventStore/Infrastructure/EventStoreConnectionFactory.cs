using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace BlogManager.Adaptor.EventStore.Infrastructure;

public class EventStoreConnectionFactory
{
    
    public async Task<IEventStoreConnection> CreateConnectionAsync(string connectionString, string streamName, string groupName)
    {
        var settings   = ConnectionSettings.Create().DisableTls();
        var connection = EventStoreConnection.Create(settings, new Uri(connectionString));

        await connection.ConnectAsync();
        await CreatePersistentSubscriptionAsync(connection, streamName, groupName);

        return connection;
    }


    private  async Task CreatePersistentSubscriptionAsync(IEventStoreConnection connection, string streamName, string groupName)
    {
        try
        {
            var settings = PersistentSubscriptionSettings.Create().MinimumCheckPointCountOf(1).StartFromBeginning().Build();
            await connection.CreatePersistentSubscriptionAsync(streamName, groupName, settings, new UserCredentials("admin", "changeit"));
        }
        catch (Exception e)
        {
        
        }
        
    }
}