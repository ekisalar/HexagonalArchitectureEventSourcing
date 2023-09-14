using System.Text;
using BlogManager.Core.Consumer;
using BlogManager.Core.Domain;
using BlogManager.Core.Events.Author;
using BlogManager.Core.Events.Blog;
using BlogManager.Core.Logger;
using BlogManager.Core.Repositories;
using EventStore.ClientAPI;

namespace BlogManager.Adaptor.EventStore.Services
{
    public class EventStoreGenerateReadModelService : IEventStoreGenerateReadModelService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly IBlogRepository       _blogRepository;
        private readonly IAuthorRepository     _authorRepository;
        private readonly IBlogManagerLogger    _logger;

        public EventStoreGenerateReadModelService(IEventStoreConnection eventStoreConnection, IBlogRepository blogRepository, IBlogManagerLogger logger, IAuthorRepository authorRepository)
        {
            _eventStoreConnection = eventStoreConnection;
            _blogRepository       = blogRepository;
            _logger               = logger;
            _authorRepository     = authorRepository;
        }

        public async Task ConnectToPersistentSubscriptionAsync(string streamName, string groupName)
        {
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(
                                                                             streamName,
                                                                             groupName,
                                                                             EventAppeared,
                                                                             autoAck: false);
        }

        public async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            try
            {
                var eventData = Encoding.UTF8.GetString(arg2.Event.Data);
                var metadata  = Encoding.UTF8.GetString(arg2.Event.Metadata);
                await HandleEvent(eventData, metadata);
                arg1.Acknowledge(arg2.Event.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing the event: {ex.Message}", ex);
            }

            arg1.Acknowledge(arg2.Event.EventId);
        }

        public async Task HandleEvent(string eventData, string metadata)
        {
            var type = Type.GetType($"{metadata}, BlogManager.Core");
            if (type == null)
            {
                _logger.LogError("Could not resolve the type: " + metadata);
                return;
            }

            var @event = Newtonsoft.Json.JsonConvert.DeserializeObject(eventData, type);

            switch (@event)
            {
                case BlogCreatedEvent blogCreatedEvent:
                    var blogToCreate = await Blog.CreateAsync(blogCreatedEvent.Id, blogCreatedEvent.AuthorId, blogCreatedEvent.Title, blogCreatedEvent.Description, blogCreatedEvent.Content);
                    await _blogRepository.AddBlogAsync(blogToCreate);
                    _logger.LogInformation($"Blog {blogToCreate.Id} created successfully to ReadDb");
                    break;

                case BlogUpdatedEvent blogUpdatedEvent:

                    var blogToUpdate = await Blog.CreateAsync(
                                                              blogUpdatedEvent.BlogDto.Id, 
                                                              blogUpdatedEvent.BlogDto.AuthorId,
                                                              blogUpdatedEvent.BlogDto.Title,
                                                              blogUpdatedEvent.BlogDto.Description,
                                                              blogUpdatedEvent.BlogDto.Content);
                    await _blogRepository.UpdateAsync(blogToUpdate);
                    _logger.LogInformation($"Blog {blogToUpdate.Id} updated successfully to ReadDb");
                    break;

                case BlogDeletedEvent blogDeletedEvent:
                    var blogToDelete = await Blog.CreateAsync(
                                                              blogDeletedEvent.BlogDto.Id, 
                                                              blogDeletedEvent.BlogDto.AuthorId,
                                                              blogDeletedEvent.BlogDto.Title,
                                                              blogDeletedEvent.BlogDto.Description,
                                                              blogDeletedEvent.BlogDto.Content);
                    await _blogRepository.DeleteBlogAsync(blogToDelete);
                    _logger.LogInformation($"Blog {blogToDelete.Id} deleted successfully to ReadDb");
                    break;

                case AuthorCreatedEvent authorCreatedEvent:
                    var authorToCreate = await Author.CreateAsync(authorCreatedEvent.Id, authorCreatedEvent.Name, authorCreatedEvent.Surname);
                    await _authorRepository.AddAuthorAsync(authorToCreate);
                    _logger.LogInformation($"Author {authorToCreate.Id} created successfully to ReadDb");
                    break;

                case AuthorUpdatedEvent authorUpdatedEvent:

                    var authorToUpdate = await Author.CreateAsync(
                                                                  authorUpdatedEvent.AuthorDto.Id, 
                                                                  authorUpdatedEvent.AuthorDto.Name, 
                                                                  authorUpdatedEvent.AuthorDto.Surname);
                    await _authorRepository.UpdateAsync(authorToUpdate);
                    _logger.LogInformation($"Author {authorToUpdate.Id} updated successfully to ReadDb");
                    break;

                case AuthorDeletedEvent authorDeletedEvent:
                    var authorToDelete = await Author.CreateAsync(
                                                                  authorDeletedEvent.AuthorDto.Id, 
                                                                  authorDeletedEvent.AuthorDto.Name, 
                                                                  authorDeletedEvent.AuthorDto.Surname);
                    await _authorRepository.DeleteAuthorAsync(authorToDelete);
                    _logger.LogInformation($"Author {authorToDelete.Id} deleted successfully to ReadDb");
                    break;


                default:
                    _logger.LogWarning("Unhandled event type: {EventType}");
                    break;
            }
        }
    }
}