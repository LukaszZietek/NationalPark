using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParkyAPI.Data;
using ParkyAPI.Mapper;
using ParkyAPI.Repo;
using ParkyAPI.Repo.IRepo;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ParkyAPI
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
            services.AddCors();
            
            
            services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddScoped<IUserRepo, UserRepo>();

            services.AddAutoMapper(typeof(ParkyMappings));


            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1,0);
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();


            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //services.AddSwaggerGen(option =>
            //{
            //    option.SwaggerDoc("ParkyOpenApiSpec",
            //        new Microsoft.OpenApi.Models.OpenApiInfo()
            //        {
            //            Title = "Parky API",
            //            Version = "1",
            //            Description = "First API",
            //            Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //            {
            //                Email = "lukaszzietekk@gmail.com",
            //                Name = "Lukasz Zietek"
            //            },
            //            License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //            {
            //                Name = "MIT License"
            //             }
            //        });
            //    //option.SwaggerDoc("ParkyOpenApiSpecTrails",
            //    //    new Microsoft.OpenApi.Models.OpenApiInfo()
            //    //    {
            //    //        Title = "Parky API",
            //    //        Version = "1",
            //    //        Description = "First API Trails",
            //    //        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            //    //        {
            //    //            Email = "lukaszzietekk@gmail.com",
            //    //            Name = "Lukasz Zietek"
            //    //        },
            //    //        License = new Microsoft.OpenApi.Models.OpenApiLicense()
            //    //        {
            //    //            Name = "MIT License"
            //    //        }
            //    //    });


            //    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //    option.IncludeXmlComments(cmlCommentFullPath);
            //});
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var desc in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                        desc.GroupName.ToUpperInvariant());
                }

                options.RoutePrefix = "";
            });

            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json","Parky API");
            //    //options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecTrails/swagger.json","Parky API Trails");
            //    options.RoutePrefix = "";
            //});

            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
