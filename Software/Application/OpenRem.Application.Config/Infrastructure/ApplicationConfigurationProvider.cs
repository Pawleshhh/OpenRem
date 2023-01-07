namespace OpenRem.Application.Config;

class ApplicationConfigurationProvider : IApplicationConfigurationProvider
{
    public IApplicationConfiguration GetConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("Config/ApplicationConfig.json")
            .Build()
            .ToApplicationConfiguration();
    }
}