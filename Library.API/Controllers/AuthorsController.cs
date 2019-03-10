using AutoMapper;
using Library.API.Entities;
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

            return Ok(authorsModel);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = LibraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorModel = Mapper.Map<AuthorDto>(author);

            return Ok(authorModel);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto request)
        {
            if (request == null) return BadRequest();

            Author authorEntity = Mapper.Map<Author>(request);

            LibraryRepository.AddAuthor(authorEntity);

            if (!LibraryRepository.Save())
            {
                throw new Exception("Ocorreu um problema ao salvar a criação do autor");
            };

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (LibraryRepository.AuthorExists(id))
            {
                return StatusCode(409);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorRepository = LibraryRepository.GetAuthor(id);

            if (authorRepository == null)
            {
                return NotFound();
            }

            LibraryRepository.DeleteAuthor(authorRepository);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Deleting author {id} failed on save.");
            }

            return NoContent();
        }
    }
}
