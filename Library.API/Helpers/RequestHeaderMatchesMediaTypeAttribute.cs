﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        public string RequestHeaderToMatch { get; }
        public string[] MediaTypes { get; }

        public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch, string[] mediaTypes)
        {
            RequestHeaderToMatch = requestHeaderToMatch ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));

            MediaTypes = mediaTypes ?? throw new ArgumentNullException(nameof(mediaTypes));
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            if (!requestHeaders.ContainsKey(RequestHeaderToMatch))
            {
                return false;
            }

            foreach (var mediaType in MediaTypes)
            {
                var mediaTypeMatches = string.Equals(requestHeaders[RequestHeaderToMatch].ToString(),
                                                        mediaType, StringComparison.OrdinalIgnoreCase);

                if (mediaTypeMatches) return true;
            }

            return false;
        }
    }
}
