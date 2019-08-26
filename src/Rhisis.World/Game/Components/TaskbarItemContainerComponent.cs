using Ether.Network.Packets;
using Rhisis.Core.Common;
using Rhisis.Core.Common.Game.Structures;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class TaskbarItemContainerComponent : ObjectContainerComponent<List<Shortcut>>
    {
        /// <summary>
        /// Gets the number of items in the item taskbar.
        /// </summary>
        public override int Count => this.Objects.Sum(x => x.Count(y => y != null && y.Type != ShortcutType.None));

        /// <summary>
        /// Gets the taskbar item max level capacity per slot.
        /// </summary>
        public int MaxLevelCapacity { get; }

        /// <summary>
        /// Creates a new <see cref="TaskbarItemContainerComponent"/> instance.
        /// </summary>
        /// <param name="maxCapacity">Taskbar max capacity (number of slot levels)</param>
        /// <param name="maxLevelCapacity">Taskbar max capacity per slot level.</param>
        public TaskbarItemContainerComponent(int maxCapacity, int maxLevelCapacity)
            : base(maxCapacity)
        {
            this.MaxLevelCapacity = maxLevelCapacity;
            this.Objects = new List<List<Shortcut>>(new List<Shortcut>[maxCapacity]);

            for (int i = 0; i < this.Objects.Count; i++)
                this.Objects[i] = new List<Shortcut>(new Shortcut[maxLevelCapacity]);
        }

        /// <inheritdoc />
        public override void Serialize(INetPacketStream packet)
        {
            packet.Write(this.Count);

            for (int level = 0; level < this.MaxCapacity; level++)
            {
                for (int slot = 0; slot < this.MaxLevelCapacity; slot++)
                {
                    if (this.Objects[level][slot] != null && this.Objects[level][slot].Type != ShortcutType.None)
                    {
                        packet.Write(level);
                        this.Objects[level][slot].Serialize(packet);
                    }
                }
            }
        }
    }
}