using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RacksStands.Framework.Auth.Authentication;

internal sealed class JwtSigningKeyConfigurator(IServiceProvider serviceProvider) : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var jwtOptions = serviceProvider.GetRequiredService<JwtOptions>();
        var factory = serviceProvider.GetRequiredService<ISigningKeyFactory>();

        options.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
        options.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
        options.TokenValidationParameters.IssuerSigningKeys =
            factory.GetValidationKeys();

    }
}
