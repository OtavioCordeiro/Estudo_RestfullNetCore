using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Library.API.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            services.AddMvc();

            var connectionString = _configuration.GetConnectionString("libraryDB");
            services.AddDbContext<LibraryContext>(x => x.UseSqlServer(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepositoryFromMemory>();
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
                app.UseExceptionHandler();
            }

            ConfigureMapper();

            //libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }

        private void ConfigureMapper()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<Author, AuthorDto>();
                });
        }
    }
}
