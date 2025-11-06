using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using Web;               // namespace where your Program class lives
using Infrastructure.Data;  // adjust if your DbContext is elsewhere

namespace PublicApiIntegrationTests
{
    [TestClass]
    public class ProgramTest
    {
        private static WebApplicationFactory<Program> _application = null!;
        public static HttpClient NewClient => _application.CreateClient();

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // Configure the in-memory host for integration testing
            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // 1️⃣  Force "Testing" environment
                    builder.UseEnvironment("Testing");

                    // 2️⃣  Replace the production DB with InMemory
                    builder.ConfigureServices(services =>
                    {
                        // Find the existing DbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                        if (descriptor != null)
                            services.Remove(descriptor);

                        // Add an in-memory database for isolation
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseInMemoryDatabase("IntegrationTests"));
                    });
                });
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _application?.Dispose();
        }
    }
}
