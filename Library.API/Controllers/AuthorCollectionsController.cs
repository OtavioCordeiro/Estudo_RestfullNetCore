using AutoMapper;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authorsCollections")]
    public class AuthorCollectionsController : Controller
    {
        public ILibraryRepository LibraryRepository { get; }

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorCollectionsForCreationDto requestCollection)
        {
            if (requestCollection == null) return BadRequest();

            var authorEntitieCollection = Mapper.Map<IEnumerable<Author>>(requestCollection.AuthorForCreationDtos);

            foreach (var authorEntity in authorEntitieCollection)
            {
                LibraryRepository.AddAuthor(authorEntity);
            }

            if (LibraryRepository.NotSave())
            {
                throw new Exception("Ocorreu um problema ao salvar a criação do autor");
            };

            var authorToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntitieCollection);

            var idsAsString = string.Join(",", authorToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { ids = idsAsString }, authorToReturn);
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids.IsEmpty()) return BadRequest();

            var authorsEntitieCollection = LibraryRepository.GetAuthors(ids);

            if (authorsEntitieCollection.Count() != ids.Count())
                return NotFound();

            var authorCollection = Mapper.Map<IEnumerable<AuthorDto>>(authorsEntitieCollection);

            return Ok(authorCollection);
        }
    }
}
