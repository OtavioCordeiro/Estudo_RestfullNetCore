using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        public ILibraryRepository LibraryRepository { get; }

        public ILogger<BooksController> Logger { get; }

        public IUrlHelper UrlHelper { get; }

        public BooksController(
            ILibraryRepository libraryRepository,
            ILogger<BooksController> logger,
            IUrlHelper urlHelper)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UrlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        [HttpGet(Name = "GetBooksForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            var books = LibraryRepository.GetBooksForAuthor(authorId);

            if (books == null)
                return NotFound();

            var booksModel = Mapper.Map<IEnumerable<BookDto>>(books);

            booksModel = booksModel.Select(book =>
            {
                book = CreateLinksForBook(book);
                return book;
            });

            var wrapper = new LinkedCollectionResourceWrapperDto<BookDto>(booksModel);

            return Ok(CreateLinksForBooks(wrapper));
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId)
        {
            var book = LibraryRepository.GetBookForAuthor(authorId, bookId);

            if (book == null)
                return NotFound();

            var bookModel = Mapper.Map<BookDto>(book);

            return Ok(CreateLinksForBook(bookModel));
        }

        [HttpPost(Name = "CreateBookForAuthor")]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody]BookForCreationDto book)
        {
            if (book == null) return BadRequest();

            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(BookForCreationDto), "O título e descrição devem ser diferentes.");
            }

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            if (LibraryRepository.AuthorNotExists(authorId)) return NotFound();

            var bookEntity = Mapper.Map<Book>(book);

            LibraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Creating a book for author {authorId} failed on save.");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor",
                                   new { authorId, bookId = bookToReturn.Id },
                                   bookToReturn);
        }

        [HttpDelete("{id}", Name = "DeleteBookForAuthor")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (LibraryRepository.AuthorNotExists(authorId))
            {
                return NotFound();
            }

            var book = LibraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
            {
                return NotFound();
            }

            LibraryRepository.DeleteBook(book);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Deleting book {id} for author {authorId} failed on save.");
            }

            Logger.LogInformation(100, $"Book {id} for author {authorId} was deleted.");

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateBookForAuthor")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto), "O título e descrição devem ser diferentes.");
            }

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            if (LibraryRepository.AuthorNotExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepository = LibraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepository == null)
            {
                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;

                LibraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (LibraryRepository.NotSave())
                {
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId, bookId = bookToReturn.Id },
                    bookToReturn);
            }

            Mapper.Map(book, bookForAuthorFromRepository);

            LibraryRepository.UpdateBookForAuthor(bookForAuthorFromRepository);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBookForAuthor")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (LibraryRepository.AuthorNotExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = LibraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                var bookDto = new BookForUpdateDto();
                patchDocument.ApplyTo(bookDto, ModelState);

                if (bookDto.Title == bookDto.Description)
                {
                    ModelState.AddModelError(nameof(BookForUpdateDto), "The provided description should be different from the title.");
                }

                TryValidateModel(bookDto);

                if (!ModelState.IsValid)
                    return new UnprocessableEntityObjectResult(ModelState);

                var bookToAdd = Mapper.Map<Book>(bookDto);
                bookToAdd.Id = id;

                LibraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (LibraryRepository.NotSave())
                {
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save.");
                }

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId, bookId = bookToReturn.Id },
                    bookToReturn);
            }

            var bookToPatch = Mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

            //patchDocument.ApplyTo(bookToPatch, ModelState);

            patchDocument.ApplyTo(bookToPatch);

            if (bookToPatch.Title == bookToPatch.Description)
            {
                ModelState.AddModelError(nameof(BookForUpdateDto), "The provided description should be different from the title.");
            }

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            LibraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        private BookDto CreateLinksForBook(BookDto book)
        {
            book.Links.Add(new LinkDto(UrlHelper.Link("GetBookForAuthor", new { bookId = book.Id }), "self", "GET"));

            book.Links.Add(new LinkDto(UrlHelper.Link("DeleteBookForAuthor", new { id = book.Id }), "delete_book", "DELETE"));

            book.Links.Add(new LinkDto(UrlHelper.Link("UpdateBookForAuthor", new { id = book.Id }), "update_book", "PUT"));

            book.Links.Add(new LinkDto(UrlHelper.Link("PartiallyUpdateBookForAuthor", new { id = book.Id }), "partially_update_book", "PATCH"));

            return book;
        }

        private LinkedCollectionResourceWrapperDto<BookDto> CreateLinksForBooks(LinkedCollectionResourceWrapperDto<BookDto> booksWrapper)
        {
            booksWrapper.Links.Add(new LinkDto(UrlHelper.Link("GetBooksForAuthor", new { }), "self", "GET"));

            return booksWrapper;
        }
    }
}
