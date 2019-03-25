using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepositoryFromDbSql : ILibraryRepository
    {
        public LibraryContext Context { get; }
        public IPropertyMappingService PropertyMappingService { get; }

        public LibraryRepositoryFromDbSql(LibraryContext context,
            IPropertyMappingService propertyMappingService)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            PropertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            Context.Authors.Add(author);

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
            return Context.Authors.Any(x => x.Id == id);
        }

        public bool AuthorNotExists(Guid id)
        {
            return !AuthorExists(id);
        }

        public void DeleteAuthor(Author author)
        {
            Context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            Context.Books.Remove(book);
        }

        public Author GetAuthor(Guid id)
        {
            return Context.Authors.FirstOrDefault(x => x.Id == id);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var collectionBeforePaging = Context.Authors.ApplySort(
                                                            authorsResourceParameters.OrderBy,
                                                            PropertyMappingService.GetPropertyMapping<AuthorDto, Author>());

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                var genreForWhereClause = authorsResourceParameters.Genre.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging.Where(x => x.Genre.ToLowerInvariant() == genreForWhereClause);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = authorsResourceParameters.SearchQuery.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging.Where(x =>
                                                                          x.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                                                                          || x.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                                                                          || x.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));

            }

            return PagedList<Author>.Create(
                collectionBeforePaging,
                authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> ids)
        {
            return Context.Authors.Where(author => ids.Contains(author.Id))
                .OrderBy(author => author.FirstName)
                .OrderBy(author => author.LastName)
                .ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return Context.Books.FirstOrDefault(b => b.Id == bookId && b.AuthorId == authorId);
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            var result = Context.Books.Where(b => b.AuthorId == authorId);

            return result.Any() ? result : null;
        }

        public bool NotSave()
        {
            return !Save();
        }

        public bool Save()
        {
            return Context.SaveChanges() > 0;
        }

        public void UpdateAuthor(Author author)
        {
            Context.Authors.Update(author);
        }

        public void UpdateBookForAuthor(Book book)
        {
            Context.Books.Update(book);
        }
    }
}
