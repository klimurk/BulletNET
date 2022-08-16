using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BulletNET
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            App app = new();
            app.InitializeComponent();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] Args) => Host
            .CreateDefaultBuilder(Args)
            .UseContentRoot(App.CurrentDirectory)
            .ConfigureAppConfiguration((host, cfg) =>
                {
                    cfg
                    .SetBasePath(App.CurrentDirectory)
                    .AddJsonFile("appsettings.json", true, true);
                })
            .ConfigureServices(App.ConfigureServices);
    }
}