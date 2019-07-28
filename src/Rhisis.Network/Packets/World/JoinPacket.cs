using Ether.Network.Packets;

namespace Rhisis.Network.Packets.World
{
    /// <summary>
    /// Defines the <see cref="JoinPacket"/> structure.
    /// </summary>
    public class JoinPacket : IPacketDeserializer
    {
        /// <summary>
        /// Gets the World Id.
        /// </summary>
        public int WorldId { get; private set; }

        /// <summary>
        /// Gets the Player Id.
        /// </summary>
        public int PlayerId { get; private set; }

        /// <summary>
        /// Gets the Authentication key.
        /// </summary>
        public int AuthenticationKey { get; private set; }

        /// <summary>
        /// Gets the player's party id.
        /// </summary>
        public int PartyId { get; private set; }

        /// <summary>
        /// Gets the player's guild id.
        /// </summary>
        public int GuildId { get; private set; }

        /// <summary>
        /// Gets the player's guild war id.
        /// </summary>
        public int GuildWarId { get; private set; }

        /// <summary>
        /// Gets the Id of multi
        /// </summary>
        /// <remarks>
        /// What is this ?
        /// </remarks>
        public int IdOfMulti { get; private set; }

        /// <summary>
        /// Gets the player's slot.
        /// </summary>
        public byte Slot { get; private set; }

        /// <summary>
        /// Gets the player's name.
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// Gets the player's account username.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the player's account password.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the messenger state.
        /// </summary>
        public int MessengerState { get; private set; }

        /// <summary>
        /// Gets the messenger count.
        /// </summary>
        public int MessengerCount { get; private set; }

        /// <inheritdoc />
        public void Deserialize(INetPacketStream packet)
        {
            this.WorldId = packet.Read<int>();
            this.PlayerId = packet.Read<int>();
            this.AuthenticationKey = packet.Read<int>();
            this.PartyId = packet.Read<int>();
            this.GuildId = packet.Read<int>();
            this.GuildWarId = packet.Read<int>();
            this.IdOfMulti = packet.Read<int>(); // what is this?
            this.Slot = packet.Read<byte>();
            this.PlayerName = packet.Read<string>();
            this.Username = packet.Read<string>();
            this.Password = packet.Read<string>();
            this.MessengerState = packet.Read<int>();
            this.MessengerCount = packet.Read<int>();
        }
    }
}
