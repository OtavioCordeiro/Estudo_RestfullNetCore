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
            var authors = LibraryRepository.GetAuthors();

            if (authors == null)
                return NotFound();

            var authorsModel = Mapper.Map<IEnumerable<AuthorDto>>(authors);

            return new JsonResult(authorsModel);
        }
    }
}
