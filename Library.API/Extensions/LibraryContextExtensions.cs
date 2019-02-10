using FizzWare.NBuilder;
using Library.API.Entities;
using System;
using System.Collections.Generic;

namespace Library.API.Extensions
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
                                    .With(x => x.DateOfBirth = GetRandomDateOfBirth())
                                .Build();
        }

        private static DateTimeOffset GetRandomDateOfBirth()
        {
            Random random = new Random();

            int year = random.Next(1980, DateTime.UtcNow.Year - 1);
            int month = random.Next(1, 13);
            int day = random.Next(1, 32);

            DateTime dateTime = new DateTime(year, month, day);

            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);

            return dateTimeOffset;
        }
    }
}
