using AutoFixture;
using Library.API.Controllers;
using Library.API.Services.Interfaces;
using NSubstitute;

namespace Library.API.Tests.AutoData
{
    public class AuthorsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
                new AuthorsController(Substitute.For<ILibraryRepository>()));
        }
    }
}
