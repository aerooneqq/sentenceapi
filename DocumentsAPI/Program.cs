using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace DocumentsAPI
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel(options => 
                   {
                       options.ListenAnyIP(6000);
                   })
                   .UseStartup<Startup>();
    }
}
