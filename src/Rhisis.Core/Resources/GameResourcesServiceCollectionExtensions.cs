using Microsoft.Extensions.DependencyInjection;

namespace Rhisis.Core.Resources
{
    /// <summary>
    /// Provides extensions related to the game resources.
    /// </summary>
    public static class GameResourcesServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the game resources to the current service collection instance.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public static void AddGameResources(this IServiceCollection services)
        {
            services.AddSingleton<IGameResources, GameResources>();
        }
    }
}
