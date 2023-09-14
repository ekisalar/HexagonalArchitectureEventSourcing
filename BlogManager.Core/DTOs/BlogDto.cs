namespace BlogManager.Core.DTOs;

public class BlogDto
{
    public Guid       Id          { get; set; }
    public Guid       AuthorId    { get; set; }
    public string     Title       { get; set; }
    public string     Description { get; set; }
    public string     Content     { get; set; }
    public AuthorDto? Author      { get; set; }
}