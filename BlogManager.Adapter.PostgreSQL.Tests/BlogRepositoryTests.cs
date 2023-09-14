using BlockManager.Tests.Shared;
using BlogManager.Adapter.PostgreSQL.DbContext;
using BlogManager.Adapter.PostgreSQL.Repositories;
using BlogManager.Core.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Adapter.PostgreSQL.Tests
{
    [TestFixture]
    public class BlogRepositoryTests
    {
        private IBlogDbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            dbContext = await DbContextFactory.CreatePostgreSqlInMemoryDbContext();
        }

        [Test]
        public async Task GetBlogByIdAsync_ReturnsBlogIfExists()
        {
            
            var blogId            = dbContext.Blogs.First().Id;
            var includeAuthorInfo = true;
            var blogRepository    = new BlogRepository(dbContext);

            
            var result = await blogRepository.GetBlogByIdAsync(blogId, includeAuthorInfo);

           
            result.Should().NotBeNull();
            result.Id.Should().Be(blogId);
        }

        [Test]
        public async Task GetBlogByIdAsync_ReturnsNullForNonExistentBlog()
        {
            
            var blogId            = Guid.NewGuid();
            var includeAuthorInfo = true;
            var blogRepository    = new BlogRepository(dbContext);

            
            var result = await blogRepository.GetBlogByIdAsync(blogId, includeAuthorInfo);

           
            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllBlogsAsync_ReturnsListOfBlogs()
        {
            
            var includeAuthorInfo = true;
            var blogRepository    = new BlogRepository(dbContext);

            
            var result = await blogRepository.GetAllBlogsAsync(includeAuthorInfo);

           
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Blog>>();
            result.All(r => r.Author != null).Should().BeTrue();
        }

        [Test]
        public async Task AddBlogAsync_AddsBlogToDatabase()
        {
            
            var blog           = await Blog.CreateAsync(Guid.NewGuid(), dbContext.Authors.First().Id, "TestTitle", "TestDescription", "TestContent");
            var blogRepository = new BlogRepository(dbContext);

            
            var result = await blogRepository.AddBlogAsync(blog);

           
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();

            // Check if the blog is actually added to the database
            var addedBlog = await dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == result.Id);
            addedBlog.Should().NotBeNull();
            addedBlog.Title.Should().Be("TestTitle");
        }

        [Test]
        public async Task UpdateAsync_UpdatesBlogInDatabase()
        {
            
            var blogRepository = new BlogRepository(dbContext);
            var blog           = await dbContext.Blogs.FirstAsync();
            
            await Blog.UpdateAsync(blog, blog.AuthorId,  "UpdatedTitle", "UpdatedDescription", "UpdatedContent");
            
            var updatedBlog = await blogRepository.UpdateAsync(blog);
            
            updatedBlog.Should().NotBeNull();
            updatedBlog.Title.Should().Be("UpdatedTitle");

            // Check if the blog is updated in the database
            var dbBlog = await dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blog.Id);
            dbBlog.Should().NotBeNull();
            dbBlog.Title.Should().Be("UpdatedTitle");
        }

        [Test]
        public async Task DeleteBlogAsync_DeletesBlogFromDatabase()
        {
            var blogRepository = new BlogRepository(dbContext);
            var blog = await dbContext.Blogs.FirstAsync();
            
            await blogRepository.DeleteBlogAsync(blog);
            
            var dbBlog = await dbContext.Blogs.FirstOrDefaultAsync(b => b.Id == blog.Id);
            dbBlog.Should().BeNull();
        }
    }
}