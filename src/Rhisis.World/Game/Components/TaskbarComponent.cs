using Ether.Network.Packets;

namespace Rhisis.World.Game.Components
{
    public class TaskbarComponent
    {
        public TaskbarAppletContainerComponent Applets { get; set; }

        public TaskbarItemContainerComponent Items { get; set; }

        public TaskbarQueueContainerComponent Queue { get; set; }

        public int ActionPoints { get; set; } = 100;

        public void Serialize(INetPacketStream packet)
        {
            this.Applets.Serialize(packet);
            this.Items.Serialize(packet);
            this.Queue.Serialize(packet);
            packet.Write(this.ActionPoints);
        }
    }
}