using AutoFixture.Idioms;
using AutoMapper;
using FluentAssertions;
using Library.API.Controllers;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Library.API.Tests.AutoData;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Library.API.Tests.Controllers
{
    public class AuthorsControllerTests
    {
        AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _controller = new AuthorsController(Substitute.For<ILibraryRepository>());
            Mapper.Initialize(
            cfg =>
            {
                cfg.CreateMap<Author, AuthorDto>();
            });
        }

        [Theory, AutoNSubstituteData]
        public void GuardTests(GuardClauseAssertion guardClauseAssertion)
        {
            guardClauseAssertion.Verify(typeof(AuthorsController).GetConstructors());
        }

        [Fact]
        public void GetAuthors_WhenCorrectly()
        {
            var result = _controller.GetAuthors();

            result.Should().BeOfType<JsonResult>();
        }
    }
}
