using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepositoryFromDbSql : ILibraryRepository
    {
        private LibraryContext _context;

        public LibraryRepositoryFromDbSql(LibraryContext context)
        {
            _context = context;
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    AddBookForAuthor(author.Id, book);
                }
            }
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                if (book.Id == null)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid id)
        {
            return _context.Authors.Any(x => x.Id == id);
        }

        public bool AuthorNotExists(Guid id)
        {
            return !AuthorExists(id);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid id)
        {
            return _context.Authors.FirstOrDefault(x => x.Id == id);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var collectionBeforePaging = _context.Authors
                                                    .OrderBy(x => x.FirstName)
                                                    .ThenBy(x => x.LastName)
                                                    .AsQueryable();

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                var genreForWhereClause = authorsResourceParameters.Genre.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging.Where(x => x.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            return PagedList<Author>.Create(
                collectionBeforePaging,
                authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> ids)
        {
            return _context.Authors.Where(author => ids.Contains(author.Id))
                .OrderBy(author => author.FirstName)
                .OrderBy(author => author.LastName)
                .ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books.FirstOrDefault(b => b.Id == bookId && b.AuthorId == authorId);
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            var result = _context.Books.Where(b => b.AuthorId == authorId);

            return result.Any() ? result : null;
        }

        public bool NotSave()
        {
            return !Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public void UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
        }

        public void UpdateBookForAuthor(Book book)
        {
            _context.Books.Update(book);
        }
    }
}
