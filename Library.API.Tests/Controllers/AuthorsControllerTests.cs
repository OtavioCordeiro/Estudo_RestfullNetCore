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
        [Theory, AutoNSubstituteData]
        public void GuardTests(GuardClauseAssertion guardClauseAssertion)
        {
            guardClauseAssertion.Verify(typeof(AuthorsController).GetConstructors());
        }

        [Theory, AutoNSubstituteData]
        public void GetAuthors_WhenCorrectly(AuthorsController sut)
        {
            ConfigureMapper();

            var result = sut.GetAuthors();

            result.Should().BeOfType<JsonResult>();
        }

        private void ConfigureMapper()
        {
            Mapper.Initialize(
            cfg =>
            {
                cfg.CreateMap<Author, AuthorDto>();
            });
        }
    }
}
