using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RacksStands.Framework.Resiliency.Options;

namespace RacksStands.Framework.Resiliency.Extensions;

public static class ResiliencyServiceCollectionExtensions
{
    public static IServiceCollection AddResiliencyPolicies(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<CircuitBreakerPolicyOptions> configCircuitBreakerPolicyOptions,
        Action<RetryPolicyOptions> configRetryPolicyOptions,
        Action<PollingPolicyOptions> configPollingPolicyOptions)
    {
        services.AddOptions<CircuitBreakerPolicyOptions>()
            .Bind(configuration.GetSection(CircuitBreakerPolicyOptions.SectionName))
            .Configure(configCircuitBreakerPolicyOptions)
            .ValidateOnStart();

        services.AddOptions<RetryPolicyOptions>()
            .Bind(configuration.GetSection(RetryPolicyOptions.SectionName))
            .Configure(configRetryPolicyOptions)
            .ValidateOnStart();

        services.AddOptions<PollingPolicyOptions>()
            .Bind(configuration.GetSection(PollingPolicyOptions.SectionName))
            .Configure(configPollingPolicyOptions)
            .ValidateOnStart();

        return services;
    }
}
