using FizzWare.NBuilder;
using System;
using System.Collections.Generic;

namespace Library.API.Entities
{
    public static class LibraryContextExtensions
    {
        public static void EnsureSeedDataForContext(this LibraryContext context)
        {
            context.Authors.RemoveRange(context.Authors);
            context.SaveChanges();

            var authors = CreateFakeDefaultAuthors();

            context.Authors.AddRange(authors);
            context.SaveChanges();
        }

        private static IEnumerable<Author> CreateFakeDefaultAuthors()
        {
            return Builder<Author>.CreateListOfSize(3)
                                .All()
                                    .With(x => x.Books = Builder<Book>.CreateListOfSize(3)
                                        .All()
                                            .With(y => y.Id = Guid.NewGuid())
                                            .With(y => y.AuthorId = x.Id)
                                            .Build())
                                    .With(x => x.Id = Guid.NewGuid())
                                .Build();
        }
    }
}
