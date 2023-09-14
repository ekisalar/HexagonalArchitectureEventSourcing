using BlogManager.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogManager.Adapter.PostgreSQL.Configuration;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.ToTable("Blogs");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.Title).HasMaxLength(150).IsRequired();
        builder.Property(b => b.Content).HasMaxLength(5000).IsRequired();
        builder.Property(b => b.Description).HasMaxLength(500).IsRequired();
        builder.HasOne(b => b.Author).WithMany(a => a.Blogs).HasForeignKey(b => b.AuthorId);
    }
}