using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Structures;
using Sylver.Network.Common;

namespace Rhisis.World.Game.Entities.Internal
{
    public class PlayerEntity : WorldEntity, IPlayerEntity
    {
        private readonly IPlayerFactory _playerFactory;

        /// <inheritdoc />
        public override WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public bool IsDead => Attributes[DefineAttributes.HP] <= 0;

        /// <inheritdoc />
        public MoverData Data { get; set; }

        /// <inheritdoc />
        public VisualAppearenceComponent VisualAppearance { get; set; }

        /// <inheritdoc />
        public PlayerDataComponent PlayerData { get; set; }

        /// <inheritdoc />
        public MovableComponent Moves { get; set; }

        /// <inheritdoc />
        public TimerComponent Timers { get; set; }

        /// <inheritdoc />
        public InventoryContainerComponent Inventory { get; set; }

        /// <inheritdoc />
        public StatisticsComponent Statistics { get; set; }

        /// <inheritdoc />
        public TradeComponent Trade { get; set; }

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
        public AttributeComponent Attributes { get; set; }

        /// <inheritdoc />
        public IBehavior Behavior { get; set; }

        /// <inheritdoc />
        public QuestDiaryComponent QuestDiary { get; set; }

        /// <inheritdoc />
        public SkillTreeComponent SkillTree { get; set; }

        /// <inheritdoc />
        public InventoryItem Hand { get; set; }

        /// <summary>
        /// Creates a new <see cref="PlayerEntity"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public PlayerEntity(IPlayerFactory playerFactory)
        {
            Moves = new MovableComponent();
            PlayerData = new PlayerDataComponent();
            Taskbar = new TaskbarComponent();
            Follow = new FollowComponent();
            Interaction = new InteractionComponent();
            Battle = new BattleComponent();
            Timers = new TimerComponent();
            Attributes = new AttributeComponent();
            QuestDiary = new QuestDiaryComponent();
            SkillTree = new SkillTreeComponent();
            Inventory = new InventoryContainerComponent();
            _playerFactory = playerFactory;
        }

        /// <summary>
        /// Dispose the <see cref="PlayerEntity"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _playerFactory.SavePlayer(this);

            base.Dispose(disposing);
        }

        public override string ToString() => Object.Name;
    }
}
