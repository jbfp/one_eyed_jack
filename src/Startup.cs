using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Sequence.AspNetCore;
using Sequence.RealTime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sequence
{
    public sealed class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => ConfigureJsonOptions(options.JsonSerializerOptions));

            services
                .AddSignalR()
                .AddJsonProtocol(options => ConfigureJsonOptions(options.PayloadSerializerOptions));

            services.AddHealthChecks();
            services.AddMemoryCache();

            if (_env.IsDevelopment())
            {
                services.AddCors();
            }

            services.AddSequence(_configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            if (_env.IsProduction())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }
            else if (_env.IsDevelopment())
            {
                app.UseCors(policy => policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:3000")
                    .AllowCredentials());

                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");

            app.UseRouting();

            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
                builder.MapHub<GameHub>("/game-hub");
            });
        }

        private static void ConfigureJsonOptions(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            var converters = options.Converters;
            converters.Add(new JsonStringEnumConverter());
            converters.Add(new GameEventConverter());
            converters.Add(new GameIdJsonConverter());
            converters.Add(new PlayerHandleJsonConverter());
            converters.Add(new PlayerIdJsonConverter());
            converters.Add(new TileJsonConverter());
        }
    }
}
