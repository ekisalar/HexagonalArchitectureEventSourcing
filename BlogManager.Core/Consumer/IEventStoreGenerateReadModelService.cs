namespace BlogManager.Core.Consumer;

public interface IEventStoreGenerateReadModelService
{
    Task ConnectToPersistentSubscriptionAsync(string streamName, string groupName);
}