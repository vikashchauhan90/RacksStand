using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Http;
using RacksStands.Framework.Auth.Security;
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

        services.TryAddScoped<ISigningKeyFactory, SigningKeyFactory>();
        services.TryAddScoped<IJwtTokenService, JwtTokenService>();
        services.TryAddScoped<IPostConfigureOptions<JwtBearerOptions>, JwtSigningKeyConfigurator>();
        services.TryAddScoped<IAccessTokenProvider, HttpAccessTokenProvider>();
        services.TryAddScoped<ITenantContext, HttpTenantContext>();
        services.TryAddScoped<IUserAgentParser, UserAgentParser>();
        services.TryAddScoped<IClientIpAddressResolver, ClientIpAddressResolver>();
        services.TryAddScoped<IHttpHeaderReader, HttpHeaderReader>();
        services.TryAddScoped<IHttpRequestMetadata, HttpRequestMetadata>();
        services.TryAddScoped<ICookieAccessor, CookieAccessor>();
        services.TryAddScoped<IRequestCorrelationService, RequestCorrelationService>();
        services.TryAddScoped<IDataProtectionService, DataProtectionService>();

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
