using Rhisis.Core.Common;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;

namespace Rhisis.World.Game.Entities
{
    public class NpcEntity : WorldEntity, INpcEntity
    {
        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Npc;

        /// <inheritdoc />
        public ItemContainerComponent[] Shop { get; set; }

        /// <inheritdoc />
        public NpcData Data { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <inheritdoc />
        public InteractionComponent Interaction { get; set; }

        /// <inheritdoc />
        public BattleComponent Battle { get; set; }

        /// <inheritdoc />
        public HealthComponent Health { get; set; }

        /// <inheritdoc />
        public AttributeComponent Attributes { get; set; }

        /// <inheritdoc />
        public MovableComponent Moves { get; set; }

        /// <inheritdoc />
        public FollowComponent Follow { get; set; }

        /// <inheritdoc />
        public TimerComponent Timers { get; set; }

        /// <summary>
        /// Creates a new <see cref="NpcEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public NpcEntity()
        {
            this.Object.Type = WorldObjectType.Mover;
            this.Timers = new TimerComponent();
            this.Interaction = new InteractionComponent();
            this.Battle = new BattleComponent();
            this.Health = new HealthComponent();
            this.Attributes = new AttributeComponent();
            this.Moves = new MovableComponent();
            this.Follow = new FollowComponent();
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Object.Name}";
    }
}
