using BlogManager.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Adapter.PostgreSQL.DbContext;

public class BlogDbContext : Microsoft.EntityFrameworkCore.DbContext, IBlogDbContext
{
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
            // this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Blog>   Blogs   { get; set; }
        public DbSet<Author> Authors { get; set; }
}