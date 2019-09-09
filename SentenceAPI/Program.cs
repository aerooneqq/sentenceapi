using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace SentenceAPI
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("https://localhost:44368")
                .UseStartup<Startup>().UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 1000;
                    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(10);
                }).UseIIS().UseIISIntegration();
    }
}
