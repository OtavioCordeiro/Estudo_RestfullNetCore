﻿using System;

namespace Library.API.Models
{
    public class AuthorDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Genre { get; set; }
    }
}
