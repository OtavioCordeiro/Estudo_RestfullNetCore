﻿using AutoMapper;
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
            if (book == null) return BadRequest();

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
    }
}