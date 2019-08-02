using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Core.Systems
{
    /// <summary>
    /// Describes the behavior of a basic system.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Gets the entity types that are allowed to execute the current system.
        /// </summary>
        WorldEntityType Type { get; }

        /// <summary>
        /// Executes the system logic for the entity passed as parameter.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="args">System parameters</param>
        void Execute(IWorldEntity entity, SystemEventArgs args);
    }
}
