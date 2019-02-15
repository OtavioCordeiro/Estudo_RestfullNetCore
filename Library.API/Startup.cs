using AutoMapper;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Models;
using Library.API.Services;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Library.API
{
    public class Startup
    {
        private static IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
            }).AddXmlSerializerFormatters();

            var connectionString = _configuration.GetConnectionString("libraryDB");
            services.AddDbContext<LibraryContext>(x => x.UseSqlServer(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepositoryFromDbSql>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
               {
                   appBuilder.Run(async context =>
                       {
                           context.Response.StatusCode = 500;
                           await context.Response.WriteAsync("Ocorreu um erro inesperado. Tente novamente mais tarde");
                       });
               });
            }

            ConfigureMapper();

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }

        private void ConfigureMapper()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<Author, AuthorDto>()
                        .ForMember(dest => dest.Name,
                                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                        .ForMember(dest => dest.Age,
                                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

                    cfg.CreateMap<Book, BookDto>();

                    cfg.CreateMap<AuthorForCreationDto, Author>();

                    cfg.CreateMap<BookForCreationDto, Book>();
                });
        }
    }
}
