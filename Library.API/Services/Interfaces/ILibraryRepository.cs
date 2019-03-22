using Library.API.Entities;
using Library.API.Helpers;
using System;
using System.Collections.Generic;

namespace Library.API.Services.Interfaces
{
    public interface ILibraryRepository
    {
        PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);

        IEnumerable<Author> GetAuthors(IEnumerable<Guid> ids);

        Author GetAuthor(Guid id);

        void AddAuthor(Author author);

        void DeleteAuthor(Author author);

        void UpdateAuthor(Author author);

        bool AuthorExists(Guid id);

        IEnumerable<Book> GetBooksForAuthor(Guid authorId);

        Book GetBookForAuthor(Guid authorId, Guid bookId);

        void AddBookForAuthor(Guid authorId, Book book);

        void UpdateBookForAuthor(Book book);

        void DeleteBook(Book book);

        bool Save();

        bool AuthorNotExists(Guid id);

        bool NotSave();
    }
}
