using Rhisis.Core.Common;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public class NpcEntity : Entity, INpcEntity
    {
        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Npc;

        /// <inheritdoc />
        public ItemContainerComponent[] Shop { get; set; }

        /// <inheritdoc />
        public NpcData Data { get; set; }

        /// <inheritdoc />
        public NpcTimerComponent Timers { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <summary>
        /// Creates a new <see cref="NpcEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public NpcEntity(IContext context)
            : base(context)
        {
            this.Object.Type = WorldObjectType.Mover;
            this.Timers = new NpcTimerComponent();
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Object.Name}";
    }
}
