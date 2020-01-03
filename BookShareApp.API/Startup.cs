using System.Net;
using BookShareApp.API.Data;
using BookShareApp.API.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using BookShareApp.API.Framework;
using AutoMapper;
using BookShareApp.API.Config;

namespace BookShareApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetDbConnection(services, UsedDb.SQLServer);
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(opt =>
                    {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });
            services.AddTransient<Seed>();
            services.AddJwtAuthenticationValidation(Configuration);
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                        p => p.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
            });
            services.AddAutoMapper();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IBookShareRepository, BookShareRepository>();
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }
            // To check later
            // app.UseHttpsRedirection();
            //seeder.SeedUsers();
            app.UseAuthentication(); // TODO check the usage of this (How does the middleware works)
            app.UseMvc();
        }

        private void SetDbConnection(IServiceCollection services, UsedDb usedDb)
        {
            switch (usedDb)
            {
                case UsedDb.SQLServer:
                    services.AddDbContext<DataContext>(x => x.UseSqlServer(Configuration.GetConnectionString("SqlServerLocalConnection")));
                    break;

                case UsedDb.SqlLite:
                    services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
                    break;
                default: break;
            }
        }

        public enum UsedDb
        {
            SQLServer,
            SqlLite
        }
    }
}
