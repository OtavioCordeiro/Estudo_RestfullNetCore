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

            var authors = new List<Author>()
            {
                new Author()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Otavio",
                    LastName = "Lopes",
                    DateOfBirth = new DateTimeOffset(new DateTime(1993, 03, 02)),
                    Genre = "Action",
                    Books = new List<Book>()
                    {
                        new Book()
                        {
                            Id = Guid.NewGuid(),
                            Title = "It",
                            Description = "asidfjo asjofjao isjfijdsafj"
                        }
                    }
                },

                new Author()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Otavio",
                    LastName = "Lopes",
                    DateOfBirth = new DateTimeOffset(new DateTime(1993, 03, 02)),
                    Genre = "Action",
                    Books = new List<Book>()
                    {
                        new Book()
                        {
                            Id = Guid.NewGuid(),
                            Title = "It",
                            Description = "asidfjo asjofjao isjfijdsafj"
                        }
                    }
                }
            };
        }
    }
}
