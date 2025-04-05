using System;
using System.IO;
using API.Extensions;
using API.Helpers;
using API.Middlewares;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config ?? throw new Exception("Configuration is NULL!");
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("=== 🔍 Debugging Configuration Keys ===");
            foreach (var item in _config.AsEnumerable())
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
            Console.WriteLine("=== 🔍 End Debugging ===");

            Console.WriteLine("EdkartConnection: " + _config.GetConnectionString("EdkartConnection"));
            Console.WriteLine("IdentityConnection: " + _config.GetConnectionString("IdentityConnection"));
            Console.WriteLine("RedisConfig:Url: " + _config["RedisConfig:Url"]);


            services.AddControllers();
            services.AddApplicationServices();
            services.AddIdentityServices(_config);
            services.AddSwaggerDocumentation();
            services.AddAutoMapper(typeof(MappingProfiles));
            // services.AddDbContext<StoreContext>(x => x.UseSqlite(_config.GetConnectionString("DefaultConnection")));
            // services.AddDbContext<AppIdentityDbContext>(x => x.UseSqlite(_config.GetConnectionString("IdentityConnection")));
            services.AddDbContext<StoreContext>(x => x.UseNpgsql(_config.GetConnectionString("EdkartConnection")));
            services.AddDbContext<AppIdentityDbContext>(x => x.UseNpgsql(_config.GetConnectionString("IdentityConnection")));

            // Get Redis configuration from appsettings.json
            var redisUrl = _config["RedisConfig:Url"];
            var redisToken = _config["RedisConfig:Token"];

            // ✅ Extract only the hostname (remove "https://")
            var redisHost = redisUrl.Replace("https://", "").TrimEnd('/'); // Example: "your-upstash-redis.upstash.io"

            // ✅ Configure Redis connection
            var options = new ConfigurationOptions
            {
                EndPoints = { $"{redisHost}:6379" },  // Use extracted host
                Password = redisToken,      // Use Upstash token as password
                Ssl = true,                 // Upstash requires SSL
                ConnectTimeout = 10000,  // 🔹 Increase connection timeout (10 sec)
                SyncTimeout = 10000,     // 🔹 Increase sync timeout (10 sec)
                AsyncTimeout = 10000,    // 🔹 Increase async timeout (10 sec)
                AbortOnConnectFail = false,
                KeepAlive = 10           // 🔹 Keep connection alive every 10 sec
            };
            services.AddSingleton<IConnectionMultiplexer>(c =>
            {
                //var configuration = ConfigurationOptions.Parse(_config.GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("https://edkart-phi.vercel.app","https://edkart-edwinjebakumars-projects.vercel.app","https://edkart-git-main-edwinjebakumars-projects.vercel.app");
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwaggerDocumentation();
            // if (env.IsDevelopment())
            // {
            //     //app.UseDeveloperExceptionPage();

            // }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            // Around line 104 in Startup.cs
            var possibleContentPaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Content"),
                Path.Combine(Directory.GetCurrentDirectory(), "API", "Content"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "Content"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "API", "Content"),
                // Additional possibilities based on the build logs
                Path.Combine("/app", "Content"),
                Path.Combine("/app", "API", "Content"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Content"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "API", "Content"),
                // If you're running from the /app/out directory
                Path.Combine("/app", "out", "Content"),
                Path.Combine("/app", "out", "API", "Content")
            };

            string contentPath = null;
            foreach (var path in possibleContentPaths)
            {
                if (Directory.Exists(path))
                {
                    contentPath = path;
                    Console.WriteLine($"Found Content directory at: {contentPath}");
                    break;
                }
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(contentPath),
                RequestPath = "/Content"
            });

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
