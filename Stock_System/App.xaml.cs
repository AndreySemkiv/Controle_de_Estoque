using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace EstoqueRolos
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; } = null!;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }
    }
}
