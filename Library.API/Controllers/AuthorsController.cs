﻿using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        public ILibraryRepository LibraryRepository { get; }
        public IUrlHelper UrlHelper { get; }

        public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
            UrlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var authors = LibraryRepository.GetAuthors(authorsResourceParameters);

            if (authors == null)
                return NotFound();

            var previousPageLink = authors.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = authors.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var authorsModel = Mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(authorsModel);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor([FromRoute]Guid id)
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

        private string CreateAuthorsResourceUri(
            AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType resourceUriType)
        {
            switch (resourceUriType)
            {
                case ResourceUriType.PreviousPage:
                    return UrlHelper.Link("GetAuthors",
                        new
                        {
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });

                case ResourceUriType.NextPage:
                    return UrlHelper.Link("GetAuthors",
                        new
                        {
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize

                        });

                default:
                    throw new Exception("ResourceUriType not implementaded");
            }
        }
    }
}
