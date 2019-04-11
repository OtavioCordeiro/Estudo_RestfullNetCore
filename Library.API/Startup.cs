using AutoMapper;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Models;
using Library.API.Services;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

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

                var jsonOutputFormatter = setup.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                jsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.marvin.author.full+json");
                jsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.marvin.authorwithdateofdeatch.full+json");

                jsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");

            }).AddXmlSerializerFormatters();

            var connectionString = _configuration.GetConnectionString("libraryDB");
            services.AddDbContext<LibraryContext>(x => x.UseSqlServer(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepositoryFromDbSql>();


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>((implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            }));

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddHttpCacheHeaders(
                (expirationModelOptions) =>
                {
                    expirationModelOptions.MaxAge = 60;
                },
                (validationModelOptions) =>
                {
                    validationModelOptions.MustRevalidate = true;
                });

            services.AddResponseCaching();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            loggerFactory.AddConsole();

            loggerFactory.AddDebug(LogLevel.Information);

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
                           var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                           if (exceptionHandlerFeature != null)
                           {
                               var logger = loggerFactory.CreateLogger("Global exception logger");
                               logger.LogError(
                                   500,
                                   exceptionHandlerFeature.Error,
                                   exceptionHandlerFeature.Error.Message);
                           }

                           context.Response.StatusCode = 500;
                           await context.Response.WriteAsync("Ocorreu um erro inesperado. Tente novamente mais tarde");
                       });
               });
            }

            ConfigureMapper();

            libraryContext.EnsureSeedDataForContext();

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

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
                                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

                    cfg.CreateMap<Book, BookDto>();

                    cfg.CreateMap<AuthorForCreationDto, Author>();

                    cfg.CreateMap<AuthorForCreationWithDateOfDeathDto, Author>();

                    cfg.CreateMap<BookForCreationDto, Book>();

                    cfg.CreateMap<BookForUpdateDto, Book>();

                    cfg.CreateMap<Book, BookForUpdateDto>();
                });
        }
    }
}
