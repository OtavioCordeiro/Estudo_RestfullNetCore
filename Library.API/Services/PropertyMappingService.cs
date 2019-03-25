﻿using Library.API.Entities;
using Library.API.Models;
using Library.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        public IList<IPropertyMapping> PropertyMappings { get; }

        public Dictionary<string, PropertyMappingValue> AuthorPropertyMapping
        {
            get
            {
                return new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                {
                    {"Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
                    {"Genre", new PropertyMappingValue(new List<string>() { "Genre" } ) },
                    {"Age", new PropertyMappingValue(new List<string>() { "DateOfBirth" }, true ) },
                    {"Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }, true ) }
                };
            }
        }

        public PropertyMappingService()
        {
            PropertyMappings.Add(new PropertyMapping<AuthorDto, Author>(AuthorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = PropertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)}>");
        }
    }
}