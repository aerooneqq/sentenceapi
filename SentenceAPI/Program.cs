using System.Net;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;


namespace SentenceAPI
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => 
                    {
                        options.Listen(IPAddress.Any, 5001, options =>
                        {
                            options.UseHttps("server.pfx", "Aero");
                        });

                        options.Listen(IPAddress.Any, 5000);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
