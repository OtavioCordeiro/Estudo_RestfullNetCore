using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Library.API.Controllers;
using Library.API.Services.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.API.Tests.AutoData
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() : base(() =>
        new Fixture().Customize(new CompositeCustomization(
            new AutoNSubstituteCustomization(),
            new AuthorsControllerCustomization())))
        { }
    }

    public class AuthorsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
                new AuthorsController(Substitute.For<ILibraryRepository>()));
        }
    }
}
