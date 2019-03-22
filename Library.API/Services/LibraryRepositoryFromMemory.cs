using FizzWare.NBuilder;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepositoryFromMemory : ILibraryRepository
    {
        IEnumerable<Author> authorsInMemory;

        public LibraryRepositoryFromMemory()
        {
            if (authorsInMemory == null)
                authorsInMemory = Builder<Author>.CreateListOfSize(3)
                    .All()
                        .With(x => x.Books = Builder<Book>.CreateListOfSize(3)
                            .All()
                                .With(y => y.Id = Guid.NewGuid())
                                .With(y => y.AuthorId = x.Id)
                                .Build())
                        .With(x => x.Id = Guid.NewGuid())
                    .Build();
        }

        public void AddAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            throw new NotImplementedException();
        }

        public bool AuthorExists(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool AuthorNotExists(Guid id)
        {
            return !AuthorExists(id);
        }

        public void DeleteAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public void DeleteBook(Book book)
        {
            throw new NotImplementedException();
        }

        public Author GetAuthor(Guid id)
        {
            return authorsInMemory.Where(x => x.Id == id).FirstOrDefault();
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var beforePaging = authorsInMemory
                                    .AsQueryable()
                                    .OrderBy(x => x.FirstName)
                                    .ThenBy(x => x.LastName);

            return PagedList<Author>.Create(
                beforePaging,
                authorsResourceParameters.PageNumber,
                authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            throw new NotImplementedException();
        }

        public bool NotSave()
        {
            return !Save();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public void UpdateBookForAuthor(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
