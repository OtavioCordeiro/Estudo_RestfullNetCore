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
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Library.API.Tests.Controllers
{
    public class AuthorsControllerTests
    {
        [Theory, AutoNSubstituteData]
        public void GuardTests(GuardClauseAssertion guardClauseAssertion)
        {
            guardClauseAssertion.Verify(typeof(AuthorsController).GetConstructors());
        }

        [Theory, AutoNSubstituteData]
        public void GetAuthors_ShouldReturnJsonResult(AuthorsController sut)
        {
            ConfigureMapper();

            var result = sut.GetAuthors();

            result.Should().BeOfType<JsonResult>();
        }

        [Theory, AutoNSubstituteData]
        public void GetAuthors_WhenNotHaveAuthors(AuthorsController sut)
        {
            ConfigureMapper();

            sut.LibraryRepository.GetAuthors().ReturnsNull();

            var result = sut.GetAuthors();

            result.Should().BeOfType<NotFoundResult>();
        }

        private void ConfigureMapper()
        {
            try
            {
                var ok = Mapper.Instance;
            }
            catch (InvalidOperationException)
            {
                Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<Author, AuthorDto>();
                });
            }
        }
    }
}
