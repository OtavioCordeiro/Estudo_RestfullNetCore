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
            var authors = Builder<Author>.CreateListOfSize(12)
                                .All()
                                    .With(x => x.Books = Builder<Book>.CreateListOfSize(3).Build())
                                    .With(x => x.DateOfBirth = GetRandomDateOfBirth()).Build();

            int i = 1;

            foreach (var author in authors)
            {
                foreach (var book in author.Books)
                {
                    book.Id = new Guid(i++.ToString().PadLeft(32, '0'));
                }
            }

            return authors;

        }

        private static DateTimeOffset GetRandomDateOfBirth()
        {
            Random random = new Random();

            int year = random.Next(1980, DateTime.UtcNow.Year - 1);
            int month = random.Next(1, 13);
            int day = random.Next(1, 29);

            DateTime dateTime = new DateTime(year, month, day);

            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime);

            return dateTimeOffset;
        }
    }
}
