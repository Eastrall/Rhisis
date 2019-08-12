using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Systems.Chat;
using Sylver.HandlerInvoker.Attributes;

namespace Rhisis.World.Handlers
{
    /// <summary>
    /// Handles every packets related to the chat system.
    /// </summary>
    [Handler]
    public sealed class ChatHandler
    {
        private readonly IChatSystem _chatSystem;

        /// <summary>
        /// Creates a new <see cref="ChatHandler"/> instance.
        /// </summary>
        /// <param name="chatSystem">Chat system.</param>
        public ChatHandler(IChatSystem chatSystem)
        {
            this._chatSystem = chatSystem;
        }

        /// <summary>
        /// Handles a chat request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        [HandlerAction(PacketType.CHAT)]
        public void OnChat(IWorldClient client, ChatPacket packet)
        {
            this._chatSystem.Chat(client.Player, packet.Message);
        }
    }
}
