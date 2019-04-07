using Library.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        public IUrlHelper UrlHelper { get; }

        public RootController(IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var links = new List<LinkDto>();

                links.Add(new LinkDto(UrlHelper.Link("GetRoot", new { }), "self", "GET"));
                links.Add(new LinkDto(UrlHelper.Link("GetAuthors", new { }), "self", "GET"));
                links.Add(new LinkDto(UrlHelper.Link("createAuthor", new { }), "create_author", "POST"));

                return Ok(links);
            }

            return NoContent();
        }
    }
}