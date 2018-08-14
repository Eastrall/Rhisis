using Ether.Network.Common;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public class PlayerEntity : Entity, IPlayerEntity
    {
        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public VisualAppearenceComponent VisualAppearance { get; set; }

        /// <inheritdoc />
        public PlayerComponent PlayerData { get; set; }

        /// <inheritdoc />
        public MovableComponent MovableComponent { get; set; }

        /// <inheritdoc />
        public ItemContainerComponent Inventory { get; set; }

        /// <inheritdoc />
        public StatisticsComponent Statistics { get; set; }

        /// <inheritdoc />
        public TradeComponent Trade { get; set; }

        /// <inheritdoc />
        public MessengerComponent Messenger { get; set; }

        /// <inheritdoc />
        public NetUser Connection { get; set; }

        /// <inheritdoc />
        public FollowComponent Follow { get; set; }

        /// <inheritdoc />
        public IBehavior<IPlayerEntity> Behavior { get; set; }

        /// <summary>
        /// Creates a new <see cref="PlayerEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public PlayerEntity(IContext context)
            : base(context)
        {
            this.Trade = new TradeComponent();
            this.Follow = new FollowComponent();
            this.Messenger = new MessengerComponent();
        }
    }
}
