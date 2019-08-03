using Rhisis.Core.Resources;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Maps.Regions;
using System.Collections.Generic;

namespace Rhisis.World.Game.Maps
{
    /// <summary>
    /// Describes the behavior of a Map instance.
    /// </summary>
    public interface IMapInstance : IMapContext
    {
        /// <summary>
        /// Gets the map name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the current map instance informations.
        /// </summary>
        WldFileInformations MapInformation { get; }

        /// <summary>
        /// Gets the map instance width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the map instance length.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the map default revival region.
        /// </summary>
        IMapRevivalRegion DefaultRevivalRegion { get; }

        /// <summary>
        /// Gets the default map layer.
        /// </summary>
        IMapLayer DefaultMapLayer { get; }

        /// <summary>
        /// Gets the map layers.
        /// </summary>
        IReadOnlyList<IMapLayer> Layers { get; }

        /// <summary>
        /// Gets the map regions.
        /// </summary>
        IReadOnlyList<IMapRegion> Regions { get; }

        /// <summary>
        /// Creates a new map layer and gives it a random id.
        /// </summary>
        /// <returns></returns>
        IMapLayer CreateMapLayer();

        /// <summary>
        /// Creates a new map layer with an id.
        /// </summary>
        /// <param name="id">Map layer id</param>
        /// <returns></returns>
        IMapLayer CreateMapLayer(int id);

        /// <summary>
        /// Gets a map layer by his id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IMapLayer GetMapLayer(int id);

        /// <summary>
        /// Deletes a map layer by his id.
        /// </summary>
        /// <param name="id"></param>
        void DeleteMapLayer(int id);

        /// <summary>
        /// Starts a context in a parallel task.
        /// </summary>
        void StartUpdateTask();

        /// <summary>
        /// Stops the context and the task.
        /// </summary>
        void StopUpdateTask();

        /// <summary>
        /// Gets the nearest revival region from a given position.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>The nearest revival region.</returns>
        IMapRevivalRegion GetNearRevivalRegion(Vector3 position);

        /// <summary>
        /// Gets the nearest revival region from a given position and if the region should be for chao mode (PK mode).
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="isChaoMode">Region is chao mode (PK mode).</param>
        /// <returns>The nearest revival region.</returns>
        IMapRevivalRegion GetNearRevivalRegion(Vector3 position, bool isChaoMode);

        /// <summary>
        /// Gets a revival region by his key.
        /// </summary>
        /// <param name="revivalKey">Revival region key.</param>
        /// <returns>Revival region matching the key.</returns>
        IMapRevivalRegion GetRevivalRegion(string revivalKey);

        /// <summary>
        /// Gets a revival region by his key and if the region should be for chao mode (PK mode).
        /// </summary>
        /// <param name="revivalKey">Revival region key.</param>
        /// <param name="isChaoMode">Region is chao mode (PK mode).</param>
        /// <returns>Revival region matching the key and the chao mode.</returns>
        IMapRevivalRegion GetRevivalRegion(string revivalKey, bool isChaoMode);

        /// <summary>
        /// Checks if the given position is enters the map bounds.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>True; if the position is in the map bounds; false otherwise.</returns>
        bool ContainsPosition(Vector3 position);
    }
}
