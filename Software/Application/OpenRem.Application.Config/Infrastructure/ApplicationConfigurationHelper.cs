namespace OpenRem.Application.Config;

public static class ApplicationConfigurationHelper
{
    public static IApplicationConfiguration ToApplicationConfiguration(this IConfiguration configuration)
    {
        return new ApplicationConfigurationAdapter(configuration);
    }
}