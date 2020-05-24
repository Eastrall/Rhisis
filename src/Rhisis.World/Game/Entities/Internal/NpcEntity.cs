using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using Rhisis.Core.Structures.Game.Quests;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Structures;
using System.Collections.Generic;

namespace Rhisis.World.Game.Entities.Internal
{
    public class NpcEntity : WorldEntity, INpcEntity
    {
        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Npc;

        /// <inheritdoc />
        public bool IsDead => Attributes[DefineAttributes.HP] <= 0;

        /// <inheritdoc />
        public ItemContainerComponent[] Shop { get; set; }

        /// <inheritdoc />
        public MoverData Data { get; set; }

        /// <inheritdoc />
        public NpcData NpcData { get; set; }

        /// <inheritdoc />
        public IEnumerable<IQuestScript> Quests { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <inheritdoc />
        public InteractionComponent Interaction { get; set; }

        /// <inheritdoc />
        public BattleComponent Battle { get; set; }

        /// <inheritdoc />
        public AttributeComponent Attributes { get; set; }

        /// <inheritdoc />
        public MovableComponent Moves { get; set; }

        /// <inheritdoc />
        public FollowComponent Follow { get; set; }

        /// <inheritdoc />
        public TimerComponent Timers { get; set; }

        /// <inheritdoc />
        public InventoryItem Hand { get; set; }

        /// <summary>
        /// Creates a new <see cref="NpcEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public NpcEntity()
        {
            Object.Type = WorldObjectType.Mover;
            Timers = new TimerComponent();
            Interaction = new InteractionComponent();
            Battle = new BattleComponent();
            Attributes = new AttributeComponent();
            Moves = new MovableComponent();
            Follow = new FollowComponent();
        }

        /// <inheritdoc />
        public override string ToString() => $"{Object.Name}";
    }
}
