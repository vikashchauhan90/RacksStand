using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Http;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Framework.Auth.Token;

namespace RacksStands.Framework.Auth;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddSingleton<ISigningKeyFactory, SigningKeyFactory>();
        services.TryAddSingleton<IJwtTokenService, JwtTokenService>();
        services.TryAddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtSigningKeyConfigurator>();
        services.TryAddSingleton<IAccessTokenProvider, HttpAccessTokenProvider>();
        services.TryAddSingleton<ITenantContext, HttpTenantContext>();
        services.TryAddSingleton<IUserAgentParser, UserAgentParser>();
        services.TryAddSingleton<IClientIpAddressResolver, ClientIpAddressResolver>();
        services.TryAddSingleton<IHttpHeaderReader, HttpHeaderReader>();
        services.TryAddSingleton<IHttpRequestMetadata, HttpRequestMetadata>();
        services.TryAddSingleton<ICookieAccessor, CookieAccessor>();
        services.TryAddSingleton<IRequestCorrelationService, RequestCorrelationService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

        return services;
    }
}
