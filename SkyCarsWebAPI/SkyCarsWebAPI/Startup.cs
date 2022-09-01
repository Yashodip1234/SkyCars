using AutoMapper;
using SkyCarsWebAPI.Infrastructure;
using FluentValidation.AspNetCore;
using SkyCarsWebAPI.Extensions;
using SkyCars.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Versioning;
using SkyCarsWebAPI.Extensions;

namespace SkyCarsWebAPI
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
            //services.Configure<DataSettings>(Configuration.GetSection("DBConnectionStrings"));
            DataSettings DBSettings = new();
            Configuration.GetSection("DBConnectionStrings").Bind(DBSettings);

            DataSettingsManager.IntiDatabaseSettings(services, DBSettings);

            DependencyRegistrar.Register(services);

            //FluentRegistration(services);

            services.Configure<AuthenticationSettings>(Configuration.GetSection("AuthenticationSettings"));
            services.Configure<EncryptionSettings>(Configuration.GetSection("PasswordEncrypt"));

            var config = Configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
            var secretKey = Encoding.ASCII.GetBytes(config.SecretKey);

            //Auto Mapper Configurations
            var mapperConfiguration = new MapperConfiguration(configure => configure.AddProfile<ApplicationMappingProfile>());
            mapperConfiguration.CreateMapper().InitializeMapper();
            services.AddMvc().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkyCarsWebAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()

                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "mypolicy",
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });
            //services.AddControllers().AddFluentValidation();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            DataSettingsManager.ApplyUpMigrations(serviceProvider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "SkyCarsWebAPI v1"));
            }
            //app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseCors("mypolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
