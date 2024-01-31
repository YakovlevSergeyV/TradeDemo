namespace Microservices.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microservices.Common.WindowsService;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class CommonProgram<TSetting, TStartup>
        where TSetting : CommonSettingsApplication
        where TStartup : CommonStartup<TSetting>
    {
        private string _url;

        public CommonProgram()
        {
            _url = string.Empty;
        }

        public CommonProgram(string getUrl)
        {
            _url = getUrl;
        }

        public void Main(string[] args)
        {
            //var isService = !(Debugger.IsAttached || args.ToList().Contains("--console"));
            var isWindowsService = args.ToList().Contains("--WindowsService");

            var builder = CreateWebHostBuilder(args.Where(arg => arg != "--WindowsService").ToArray());

            if (isWindowsService)
            {
                var fileName = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(fileName);
                builder.UseContentRoot(pathToContentRoot);
            }

            var host = builder.Build();

            if (isWindowsService)
            {
                host.RunAsCustomService();
            }
            else
            {
                try
                {
                    host.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var url = _url;
            if (args.Length > 0)
            {
                url = args[0];
            }

            if (string.IsNullOrEmpty(url))
            {
                return WebHost.CreateDefaultBuilder(args)
                    //.ConfigureAppConfiguration((context, builder) =>
                    //{
                    //    var env = context.HostingEnvironment;
                    //    var d = env.EnvironmentName;

                    //    builder.AddJsonFile("C:\\Temp\\Databases\\appsettings.json", false, true);
                    //})
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<TStartup>();
            }

            return WebHost.CreateDefaultBuilder(args)

                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<TStartup>()
                .UseUrls(url);
        }

        private static string GetCurrentDirectory()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var currentPath = Path.GetDirectoryName(currentAssembly.Location);
            if (currentPath == null)
            {
                throw new InvalidOperationException(
                    $"Невозможно определить текущий каталог сборки '{currentAssembly.GetName().Name}'.");
            }
            return currentPath;
        }
    }
}
