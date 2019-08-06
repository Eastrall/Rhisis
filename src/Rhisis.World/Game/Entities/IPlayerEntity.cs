using Ether.Network.Common;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Core;

namespace Rhisis.World.Game.Entities
{
    public interface IPlayerEntity : IWorldEntity, IMovableEntity, ILivingEntity
    {
        /// <summary>
        /// Gets or sets the player's visual appearance component.
        /// </summary>
        VisualAppearenceComponent VisualAppearance { get; set; }

        /// <summary>
        /// Gets or sets the player's data component.
        /// </summary>
        PlayerDataComponent PlayerData { get; set; }

        /// <summary>
        /// Gets or sets the player's inventory.
        /// </summary>
        ItemContainerComponent Inventory { get; set; }

        /// <summary>
        /// Gets or sets the player's trade component.
        /// </summary>
        TradeComponent Trade { get; set; }

        /// <summary>
        /// Gets or sets the player's party component.
        /// </summary>
        PartyComponent Party { get; set; }

        /// <summary>
        /// Gets or sets the player's taskbar component.
        /// </summary>
        TaskbarComponent Taskbar { get; set; }

        /// <summary>
        /// Gets or sets the player's statistics component.
        /// </summary>
        StatisticsComponent Statistics { get; set; }

        /// <summary>
        /// Gets or sets the player's connection.
        /// </summary>
        INetUser Connection { get; set; }
    }
}
