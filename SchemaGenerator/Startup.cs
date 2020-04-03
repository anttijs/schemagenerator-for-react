using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExampleDbLib;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
namespace SchemaGenerator
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
            var corsArray = Configuration.GetSection("AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                builder =>
                {
                    builder.WithOrigins(corsArray)
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Helpers.MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Authority = $"{Configuration["AzureAdB2C:IdentityProvider"]}/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:Policy"]}/v2.0/";
                    jwtOptions.Audience = Configuration["AzureAdB2C:ClientId"];
                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = AuthenticationFailed,
                        OnMessageReceived = MessageReceivedAsync,
                    };
                });

            services.AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            services.AddHttpContextAccessor();
            services.AddDbContext<ExampleDbContext>(options =>
                //options.UseLazyLoadingProxies()
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseCors("AllowedOrigins");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseFileServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
        
        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            //var s = $"AuthenticationFailed: {arg.Exception.Message}";
            //arg.Response.ContentLength = s.Length;
            //arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
        private Task MessageReceivedAsync(MessageReceivedContext arg)
        {
            var authHeader = arg.HttpContext.Request.Headers["Authorization"];
            // 7 = (Bearer + " ").Length
            if (StringValues.IsNullOrEmpty(authHeader) == false && authHeader.ToString().Length > 7) 
            {
                var token = authHeader.ToString().Substring(7);
            }
            return Task.FromResult(0);
        }
    }

}
