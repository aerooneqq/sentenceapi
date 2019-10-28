using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SentenceAPI
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:7000")
                .UseStartup<Startup>();
    }
}
