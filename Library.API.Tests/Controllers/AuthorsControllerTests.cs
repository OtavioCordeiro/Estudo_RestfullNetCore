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
using System.Linq;
using System.Text;
using Xunit;

namespace Library.API.Tests.Controllers
{
    public class AuthorsControllerTests
    {
        public AuthorsControllerTests()
        {
            ConfigureTests();
        }

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

        private void ConfigureTests()
        {
            var methodName = GetMethodName();

            if (methodName.Equals("GuardTests", StringComparison.InvariantCulture) == false)
            {
                ConfigureMapper();
            }
        }

        private string GetMethodName()
        {
            return GetType().GetMethods()
                            .Select(m => m.Name)
                            .FirstOrDefault();
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
