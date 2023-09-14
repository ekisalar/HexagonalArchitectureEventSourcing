namespace BlogManager.Adaptor.EventStore.Infrastructure;

public class EventStoreConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public string StreamName       { get; set; } = string.Empty;
    public string GroupName        { get; set; } = string.Empty;
}