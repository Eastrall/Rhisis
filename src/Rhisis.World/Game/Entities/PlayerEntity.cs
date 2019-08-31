using Ether.Network.Common;
using Rhisis.Core.Common;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Systems.PlayerData;

namespace Rhisis.World.Game.Entities
{
    public class PlayerEntity : WorldEntity, IPlayerEntity
    {
        private readonly IPlayerDataSystem _playerDataSystem;

        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public VisualAppearenceComponent VisualAppearance { get; set; }

        /// <inheritdoc />
        public PlayerDataComponent PlayerData { get; set; }

        /// <inheritdoc />
        public MovableComponent Moves { get; set; }

        /// <inheritdoc />
        public TimerComponent Timers { get; set; }

        /// <inheritdoc />
        public ItemContainerComponent Inventory { get; set; }

        /// <inheritdoc />
        public StatisticsComponent Statistics { get; set; }

        /// <inheritdoc />
        public TradeComponent Trade { get; set; }

        /// <inheritdoc />
        public PartyComponent Party { get; set; }

        /// <inheritdoc />
        public TaskbarComponent Taskbar { get; set; }

        /// <inheritdoc />
        public INetUser Connection { get; set; }

        /// <inheritdoc />
        public FollowComponent Follow { get; set; }

        /// <inheritdoc />
        public InteractionComponent Interaction { get; set; }

        /// <inheritdoc />
        public BattleComponent Battle { get; set; }

        /// <inheritdoc />
        public HealthComponent Health { get; set; }

        /// <inheritdoc />
        public AttributeComponent Attributes { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <summary>
        /// Creates a new <see cref="PlayerEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public PlayerEntity(IPlayerDataSystem playerDataSystem)
        {
            this.Moves = new MovableComponent();
            this.PlayerData = new PlayerDataComponent();
            this.Party = new PartyComponent();
            this.Taskbar = new TaskbarComponent();
            this.Follow = new FollowComponent();
            this.Interaction = new InteractionComponent();
            this.Battle = new BattleComponent();
            this.Health = new HealthComponent();
            this.Timers = new TimerComponent();
            this.Attributes = new AttributeComponent();
            this._playerDataSystem = playerDataSystem;
        }

        /// <summary>
        /// Dispose the <see cref="PlayerEntity"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this._playerDataSystem.SavePlayer(this);
            base.Dispose(disposing);
        }

        public override string ToString() => this.Object.Name;
    }
}
