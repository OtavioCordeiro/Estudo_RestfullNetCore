using AutoFixture;
using Library.API.Entities;
using Library.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Library.API.Tests.AutoData
{
    internal class LibraryRepositoryFromDbSqlCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
                new LibraryRepositoryFromDbSql(
                    Substitute.For<LibraryContext>(
                        Substitute.For<DbContextOptions<LibraryContext>>(
                            Substitute.For<IReadOnlyDictionary<LibraryRepositoryFromDbSql, IDbContextOptionsExtension>>()))));
        }
    }
}