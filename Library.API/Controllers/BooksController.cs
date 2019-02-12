using AutoMapper;
using Library.API.Models;
using Library.API.Services.Interfaces;
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

        [HttpGet("{bookId}")]
        public IActionResult GetBooks(Guid authorId, Guid bookId)
        {
            var book = LibraryRepository.GetBookForAuthor(authorId, bookId);

            if (book == null)
                return NotFound();

            var bookModel = Mapper.Map<BookDto>(book);

            return Ok(bookModel);
        }
    }
}
