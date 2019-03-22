using AutoFixture.Idioms;
using AutoMapper;
using FluentAssertions;
using Library.API.Controllers;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Tests.AutoData;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ReturnsExtensions;
using System;
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
        public void GetAuthors_ShouldReturnJsonResult(AuthorsController sut, AuthorsResourceParameters authorsResourceParameters)
        {
            ConfigureMapper();

            var result = sut.GetAuthors(authorsResourceParameters);

            result.Should().BeOfType<JsonResult>();
        }

        [Theory, AutoNSubstituteData]
        public void GetAuthors_WhenNotHaveAuthors(AuthorsController sut, AuthorsResourceParameters authorsResourceParameters)
        {
            ConfigureMapper();

            sut.LibraryRepository.GetAuthors(authorsResourceParameters).ReturnsNull();

            var result = sut.GetAuthors(authorsResourceParameters);

            result.Should().BeOfType<NotFoundResult>();
        }

        private void ConfigureMapper()
        {
            try
            {
                Mapper.AssertConfigurationIsValid();
            }
            catch (InvalidOperationException)
            {
                InitializeMapper();
            }
        }

        private static void InitializeMapper()
        {
            Mapper.Initialize(
                            cfg =>
                            {
                                cfg.CreateMap<Author, AuthorDto>()
                                    .ForMember(dest => dest.Name,
                                                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                                    .ForMember(dest => dest.Age,
                                                opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
                            });
        }
    }
}
