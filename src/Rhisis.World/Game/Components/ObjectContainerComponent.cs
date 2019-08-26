using Ether.Network.Packets;
using System.Collections.Generic;

namespace Rhisis.World.Game.Components
{
    public class ObjectContainerComponent<TObject>
        where TObject : class
    {
        /// <summary>
        /// Gets the maximum capacity of the container.
        /// </summary>
        public int MaxCapacity { get; }

        /// <summary>
        /// Gets the objects collection of the container.
        /// </summary>
        public List<TObject> Objects { get; protected set; }

        /// <summary>
        /// Gets the amount of object in the container.
        /// </summary>
        public virtual int Count { get; }

        /// <summary>
        /// Creates a new <see cref="ObjectContainerComponent{TObject}"/> instance.
        /// </summary>
        /// <param name="maxCapacity">Maximum capacity.</param>
        protected ObjectContainerComponent(int maxCapacity)
        {
            this.MaxCapacity = maxCapacity;
        }

        /// <summary>
        /// Serialize the object container.
        /// </summary>
        /// <param name="packet">Packet stream.</param>
        public virtual void Serialize(INetPacketStream packet)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// Checks if the given slot is available.
        /// </summary>
        /// <param name="slotIndex">Slot index.</param>
        /// <returns>True if the slot is available, false otherwise.</returns>
        public virtual bool IsSlotAvailable(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= this.MaxCapacity)
                return false;

            return this.Objects[slotIndex] == null;
        }
    }
}