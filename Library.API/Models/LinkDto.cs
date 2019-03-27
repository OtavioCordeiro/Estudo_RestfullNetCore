using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public class LinkDto
    {
        public string Href { get; }
        public string Rel { get; }
        public string Method { get; }

        public LinkDto(string href, string rel, string method)
        {
            Href = href ?? throw new ArgumentNullException(nameof(href));
            Rel = rel ?? throw new ArgumentNullException(nameof(rel));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }
    }
}
