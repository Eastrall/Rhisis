using Rhisis.World.Game.Entities;

namespace Rhisis.World.Game.Components
{
    public class InteractionComponent
    {
        /// <summary>
        /// The TargetEntity to interact with.
        /// </summary>
        public IWorldEntity TargetEntity { get; set; }
    }
}