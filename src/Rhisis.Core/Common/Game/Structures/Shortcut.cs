using Ether.Network.Packets;

namespace Rhisis.Core.Common.Game.Structures
{
    public sealed class Shortcut
    {
        /// <summary>
        /// Gets the shortcut slot index.
        /// </summary>
        public int SlotIndex { get; }

        /// <summary>
        /// Gets the shortcut type.
        /// </summary>
        public ShortcutType Type { get; }

        /// <summary>
        /// Gets the shortcut object unique id.
        /// </summary>
        public uint ObjectId { get; }

        /// <summary>
        /// Gets the shortcut object type.
        /// </summary>
        public ShortcutObjectType ObjectType { get; }

        /// <summary>
        /// Gets the shortcut object index.
        /// </summary>
        public uint ObjectIndex { get; }

        /// <summary>
        /// Gets the shortcut user id.
        /// </summary>
        public uint UserId { get; }

        /// <summary>
        /// Gets the shortcut additionnal data.
        /// </summary>
        public uint ObjectData { get; }

        /// <summary>
        /// Gets the shortcut text.
        /// </summary>
        public string Text { get; }

        public Shortcut(int slotIndex, ShortcutType type, uint objId, ShortcutObjectType shortcutObjectType, uint objIndex, uint userId, uint objData, string text)
        {
            SlotIndex = slotIndex;
            Type = type;
            ObjectId = objId;
            ObjectType = shortcutObjectType;
            ObjectIndex = objIndex;
            UserId = userId;
            ObjectData = objData;
            Text = text;
        }

        public void Serialize(INetPacketStream packet)
        {
            packet.Write(SlotIndex);
            packet.Write((uint)Type);
            packet.Write(ObjectId);
            packet.Write((uint)ObjectType);
            packet.Write(ObjectIndex);
            packet.Write(UserId);
            packet.Write(ObjectData);

            if (Type == ShortcutType.Chat)
                packet.Write(Text);
        }
    }
}