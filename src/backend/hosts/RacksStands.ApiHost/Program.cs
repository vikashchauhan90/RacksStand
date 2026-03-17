
using RacksStands.ApiHost.Extensions;

await WebApplication
    .CreateBuilder(args)
    .AddServices()
    .ConfigurePipeline()
    .RunAsync();
