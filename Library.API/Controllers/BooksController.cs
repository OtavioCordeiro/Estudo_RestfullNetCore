using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        public BooksController(ILibraryRepository libraryRepository)
        {
            LibraryRepository = libraryRepository ?? throw new ArgumentNullException(nameof(libraryRepository));
        }

        [HttpGet]
        public IActionResult GetBooks(Guid authorId)
        {
            var books = LibraryRepository.GetBooksForAuthor(authorId);

            if (books == null)
                return NotFound();

            var booksModel = Mapper.Map<IEnumerable<BookDto>>(books);

            return Ok(booksModel);
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public IActionResult GetBooks(Guid authorId, Guid bookId)
        {
            var book = LibraryRepository.GetBookForAuthor(authorId, bookId);

            if (book == null)
                return NotFound();

            var bookModel = Mapper.Map<BookDto>(book);

            return Ok(bookModel);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody]BookForCreationDto book)
        {
            if (book?.Title == null) return BadRequest();

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

        [HttpDelete("{id}")]
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

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

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

        [HttpPatch("{id}")]
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
                patchDocument.ApplyTo(bookDto);

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

            patchDocument.ApplyTo(bookToPatch);

            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            LibraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (LibraryRepository.NotSave())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}
