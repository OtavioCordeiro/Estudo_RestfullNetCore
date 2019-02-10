using AutoMapper;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        public ILibraryRepository LibraryRepository { get; }

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            throw new Exception();

            var authors = LibraryRepository.GetAuthors();

            if (authors == null)
                return NotFound();

            var authorsModel = Mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsModel);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = LibraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorModel = Mapper.Map<AuthorDto>(author);

            return Ok(authorModel);
        }
    }
}
