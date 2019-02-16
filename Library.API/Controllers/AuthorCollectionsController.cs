using AutoMapper;
using Library.API.Entities;
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

            var authorEntities = Mapper.Map<IEnumerable<Author>>(requestCollection.AuthorForCreationDtos);

            foreach (var authorEntity in authorEntities)
            {
                LibraryRepository.AddAuthor(authorEntity);
            }

            if (LibraryRepository.NotSave())
            {
                throw new Exception("Ocorreu um problema ao salvar a criação do autor");
            };

            var authorToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok();
        }
    }
}
