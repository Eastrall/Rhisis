using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Maps.Regions;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Game.Entities.Internal
{
    /// <summary>
    /// Describes the Monster entity.
    /// </summary>
    public class MonsterEntity : WorldEntity, IMonsterEntity
    {
        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Monster;

        /// <inheritdoc />
        public bool IsDead => Attributes[DefineAttributes.HP] <= 0;

        /// <inheritdoc />
        public IMapRespawnRegion Region { get; set; }

        /// <inheritdoc />
        public MovableComponent Moves { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <inheritdoc />
        public TimerComponent Timers { get; set; }

        /// <inheritdoc />
        public FollowComponent Follow { get; set; }

        /// <inheritdoc />
        public InteractionComponent Interaction { get; set; }

        /// <inheritdoc />
        public BattleComponent Battle { get; set; }

        /// <inheritdoc />
        public StatisticsComponent Statistics { get; set; }

        /// <inheritdoc />
        public AttributeComponent Attributes { get; set; }

        /// <inheritdoc />
        public MoverData Data { get; set; }

        /// <inheritdoc />
        public InventoryItem Hand { get; set; }

        /// <summary>
        /// Creates a new <see cref="MonsterEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public MonsterEntity()
        {
            Moves = new MovableComponent();
            Timers = new TimerComponent();
            Follow = new FollowComponent();
            Interaction = new InteractionComponent();
            Battle = new BattleComponent();
            Attributes = new AttributeComponent();
        }

        public override string ToString() => Object.Name;
    }
}
