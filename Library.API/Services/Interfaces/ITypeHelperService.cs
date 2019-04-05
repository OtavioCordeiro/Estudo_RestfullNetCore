namespace Library.API.Services.Interfaces
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
