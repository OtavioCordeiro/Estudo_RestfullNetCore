using AutoMapper;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        public ILibraryRepository LibraryRepository { get; }
        public IUrlHelper UrlHelper { get; }
        public IPropertyMappingService PropertyMappingService { get; }
        public ITypeHelperService TypeHelperService { get; }

        public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
            UrlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            PropertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            TypeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!PropertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
                return BadRequest();

            if (!TypeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields)) { return BadRequest(); }

            var authorsFromRepo = LibraryRepository.GetAuthors(authorsResourceParameters);

            if (authorsFromRepo == null)
                return NotFound();

            var authorsModel = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalPages = authorsFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForAuthors(authorsResourceParameters, authorsFromRepo.HasNext, authorsFromRepo.HasPrevious);

                var shapedAuthors = authorsModel.ShapeData(authorsResourceParameters.Fields);

                var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
                 {
                     var authorAsDictionary = author as IDictionary<string, object>;
                     var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["Id"], authorsResourceParameters.Fields);

                     authorAsDictionary.Add("links", authorLinks);

                     return authorAsDictionary;
                 });

                var linkedCollectionResource = new
                {
                    values = shapedAuthorsWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;

                var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage,
                    totalPages = authorsFromRepo.TotalPages,
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                return Ok(authorsModel.ShapeData(authorsResourceParameters.Fields));
            }
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor([FromRoute]Guid id, [FromQuery]string fields)
        {
            var authorFromRepo = LibraryRepository.GetAuthor(id);

            if (authorFromRepo == null)
                return NotFound();

            var authorFromModel = Mapper.Map<AuthorDto>(authorFromRepo);

            var links = CreateLinksForAuthor(id, fields);

            var linkedResourceToReturn = authorFromModel.ShapeData(fields) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateAuthor")]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] {
                "application/vnd.marvin.author.full+json",
                "application/vnd.marvin.author.full+xml"})]
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

            var links = CreateLinksForAuthor(authorToReturn.Id, null);

            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateAuthorWithDateOfDeath")]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] {
                "application/vnd.marvin.authorwithdateofdeath.full+json",
                "application/vnd.marvin.authorwithdateofdeath.full+xml"})]
        public IActionResult CreateAuthorWithDateOfDeath([FromBody] AuthorForCreationWithDateOfDeathDto author)
        {
            if (author == null) return BadRequest();

            Author authorEntity = Mapper.Map<Author>(author);

            LibraryRepository.AddAuthor(authorEntity);

            if (!LibraryRepository.Save())
            {
                throw new Exception("Ocorreu um problema ao salvar a criação do autor");
            };

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinksForAuthor(authorToReturn.Id, null);

            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
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

        [HttpDelete("{id}", Name = "DeleteAuthor")]
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

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
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
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });

                case ResourceUriType.NextPage:
                    return UrlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize

                        });

                case ResourceUriType.Current:
                    return UrlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        });

                default:
                    throw new Exception("ResourceUriType not implementaded");
            }
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(UrlHelper.Link("GetAuthor", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(new LinkDto(UrlHelper.Link("GetAuthor", new { id, fields }), "self", "GET"));
            }

            links.Add(new LinkDto(UrlHelper.Link("DeleteAuthor", new { id }), "delete_author", "DELETE"));

            links.Add(new LinkDto(UrlHelper.Link("CreateBookForAuthor", new { authorId = id }), "create_book_for_author", "POST"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}
