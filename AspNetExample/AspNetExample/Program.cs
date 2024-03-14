
using AspNetExample.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMemoryCache(); // to cache the token
            builder.Services.AddHttpClient(); // to request the token

            // register a token service that is able to cache the token.
            builder.Services.AddScoped(provider =>
            {
                var client = provider.GetRequiredService<IHttpClientFactory>();
                var cache = provider.GetRequiredService<IMemoryCache>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var clientId = configuration["AzureB2C:ClientId"] ?? throw new Exception("Missing client id");
                var clientSecret = configuration["AzureB2C:ClientSecret"] ?? throw new Exception("Missing client secret");
                var scope = configuration["AzureB2C:Scope"] ?? throw new Exception("Missing scope");
                var authority = configuration["AzureB2C:Authority"] ?? throw new Exception("Missing authority");

                return new TokenService(client, cache, clientId, clientSecret, scope, authority);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
