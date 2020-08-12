using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ExtJwtTokenValidation.Util.Auth
{
    public class ConfigurationHelper
    {
        public IConfiguration Config { get; set; }

        public ConfigurationHelper()
        {
            IHostingEnvironment env = JwtValidationProvider.ServiceProvider.GetRequiredService<IHostingEnvironment>();
            Config = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
             .Build();
        }

        public T GetAppSettings<T>(string key) where T : class, new()
        {
            var appconfig = new ServiceCollection()
             .AddOptions()
             .Configure<T>(Config.GetSection(key))
             .BuildServiceProvider()
             .GetService<IOptions<T>>()
             .Value;
            return appconfig;
        }
    }
}