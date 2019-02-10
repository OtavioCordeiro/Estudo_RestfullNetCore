using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Library.API.Tests.AutoData
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() : base(() =>
        new Fixture().Customize(new CompositeCustomization(
            new AutoNSubstituteCustomization(),
            new AuthorsControllerCustomization(),
            new LibraryRepositoryFromDbSqlCustomization())))
        { }
    }

    
}
