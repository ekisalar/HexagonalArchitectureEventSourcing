using FluentValidation;

namespace BlogManager.Core.Domain;

public class Author
{
    public Guid        Id      { get; private set; }
    public string      Name    { get; private set; }
    public string      Surname { get; private set; }
    public IList<Blog> Blogs   { get; private set; }

    public Author(Guid id,string name, string surname)
    {
        Id      = id;
        Name    = name;
        Surname = surname;
    }

    public static async Task<Author> CreateAsync(Guid id, string name, string surname)
    {

        var authorToCreate   = new Author(id, name, surname);
        var validator        = new CreateAuthorValidator();
        var validationResult = await validator.ValidateAsync(authorToCreate);
        if (validationResult.IsValid)
            return authorToCreate;
        throw new Exception(validationResult.Errors.ToString());
    }
    
    
    public static async Task<Author> UpdateAsync(Author authorToUpdate, string name, string surname)
    {
        authorToUpdate.Name    = name;
        authorToUpdate.Surname = surname;
        var validator        = new UpdateAuthorValidator();
        var validationResult = await validator.ValidateAsync(authorToUpdate);
        if (validationResult.IsValid)
            return authorToUpdate;
        throw new Exception(validationResult.Errors.ToString());
    }

    public static async Task<Author> DeleteAsync(Author authorToDelete)
    {
        var validator        = new DeleteAuthorValidator();
        var validationResult = await validator.ValidateAsync(authorToDelete);
        if (validationResult.IsValid)
            return authorToDelete;
        throw new Exception(validationResult.Errors.ToString());
    }

    private class CreateAuthorValidator : AbstractValidator<Author>
    {
        public CreateAuthorValidator()
        {
            RuleFor(author => author.Name).NotEmpty().MaximumLength(100);
            RuleFor(author => author.Surname).NotEmpty().MaximumLength(100);
        }
    }

    private class UpdateAuthorValidator : AbstractValidator<Author>
    {
        public UpdateAuthorValidator()
        {
            RuleFor(author => author.Id).NotEmpty();
            RuleFor(author => author.Name).NotEmpty().MaximumLength(100);
            RuleFor(author => author.Surname).NotEmpty().MaximumLength(100);
        }
    }

    private class DeleteAuthorValidator : AbstractValidator<Author>
    {
        public DeleteAuthorValidator()
        {
            RuleFor(author => author.Id).NotEmpty();
        }
    }
}