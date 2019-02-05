using Library.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services.Interfaces
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();

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
    }
}
