using Rhisis.Core.Structures;

namespace Rhisis.World.Systems.Teleport
{
    public class TeleportEventArgs
    {
        /// <summary>
        /// Gets the target map id.
        /// </summary>
        public int MapId { get; }

        /// <summary>
        /// Gets the target X position.
        /// </summary>
        public float PositionX { get; }

        /// <summary>
        /// Gets the target Y position.
        /// </summary>
        public float? PositionY { get; }

        /// <summary>
        /// Gets the target Z position.
        /// </summary>
        public float PositionZ { get; }

        /// <summary>
        /// Gets the target angle.
        /// </summary>
        public float Angle { get; }

        /// <summary>
        /// Creates a new <see cref="TeleportEventArgs"/> instance.
        /// </summary>
        /// <param name="mapId">Target Map Id.</param>
        /// <param name="position">Teleport position.</param>
        public TeleportEventArgs(int mapId, Vector3 position)
            : this(mapId, position.X, position.Z, position.Y)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TeleportEventArgs"/> instance.
        /// </summary>
        /// <param name="mapId">Target Map Id.</param>
        /// <param name="positionX">Target X position.</param>
        /// <param name="positionZ">Target Z position.</param>
        public TeleportEventArgs(int mapId, float positionX, float positionZ)
            : this(mapId, positionX, positionZ, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TeleportEventArgs"/> instance.
        /// </summary>
        /// <param name="mapId">Target Map Id.</param>
        /// <param name="positionX">Target X position.</param>
        /// <param name="positionZ">Target Z position.</param>
        /// <param name="positionY">Target Y position.</param>
        public TeleportEventArgs(int mapId, float positionX, float positionZ, float? positionY)
            : this(mapId, positionX, positionZ, positionY, 0)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TeleportEventArgs"/> instance.
        /// </summary>
        /// <param name="mapId">Target Map Id.</param>
        /// <param name="positionX">Target X position.</param>
        /// <param name="positionZ">Target Z position.</param>
        /// <param name="positionY">Target Y position.</param>
        /// <param name="angle">Target angle.</param>
        public TeleportEventArgs(int mapId, float positionX, float positionZ, float? positionY, float angle)
        {
            this.MapId = mapId;
            this.PositionX = positionX;
            this.PositionZ = positionZ;
            this.PositionY = positionY;
            this.Angle = angle;
        }
    }
}
