using System.Collections.Generic;

namespace Library.API.Models
{
    public class AuthorCollectionsForCreationDto
    {
        public IEnumerable<AuthorForCreationDto> AuthorForCreationDtos { get; set; }
    }
}
