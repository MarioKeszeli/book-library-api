using BookLibrary.Domain.Repositories;
using BookLibrary.Domain.Services;
using BookLibrary.Infrastructure.Repositories;
using BookLibrary.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(BookLibrary.FunctionApps.Startup))]

namespace BookLibrary.FunctionApps;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config =
                new ConfigurationBuilder()
                    .SetBasePath(builder.GetContext().ApplicationRootPath)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

        builder.Services.AddDbContextFactory<CosmosDbContext>(optionsBuilder =>
          optionsBuilder
            .UseCosmos(
              connectionString: config.GetConnectionString("CosmosDB"),
              databaseName: "BookLibraryDB",
              cosmosOptionsAction: options =>
              {
                  options.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
                  options.MaxRequestsPerTcpConnection(16);
                  options.MaxTcpConnectionsPerEndpoint(32);
              }));

        builder.Services.AddScoped<CosmosDbContext>();

        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IBorrowingService, BorrowingService>();

        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IBorrowingRepository, BorrowingRepository>();
    }
}
