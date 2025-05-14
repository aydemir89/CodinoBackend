using Microsoft.Extensions.Configuration;

namespace Codino_UserCredential.Core.Functions
{
    public class LoadTest
    {
        public static bool IsLoadTestMode()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            bool isEnabled = configuration.GetSection("LoadTest:IsEnabled").Get<bool>();
            return isEnabled;
            
        }
    }
}