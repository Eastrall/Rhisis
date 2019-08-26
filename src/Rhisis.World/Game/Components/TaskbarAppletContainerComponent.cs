using Ether.Network.Packets;
using Rhisis.Core.Common;
using Rhisis.Core.Common.Game.Structures;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class TaskbarAppletContainerComponent : ObjectContainerComponent<Shortcut>
    {
        /// <summary>
        /// Gets the number of applets in the container.
        /// </summary>
        public override int Count => this.Objects.Count(x => x != null && x.Type != ShortcutType.None);

        /// <summary>
        /// Creates a new <see cref="TaskbarAppletContainerComponent"/> instance.
        /// </summary>
        /// <param name="maxCapacity">Taskbar applet max capacity.</param>
        public TaskbarAppletContainerComponent(int maxCapacity)
            : base(maxCapacity)
        {
            this.Objects = new List<Shortcut>(new Shortcut[maxCapacity]);
        }

        /// <inheritdoc />
        public override void Serialize(INetPacketStream packet)
        {
            packet.Write(this.Count);

            for (int i = 0; i < this.MaxCapacity; i++)
            {
                if (this.Objects[i] != null && this.Objects[i].Type != ShortcutType.None)
                {
                    this.Objects[i].Serialize(packet);
                }
            }
        }
    }
}