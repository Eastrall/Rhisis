using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets
{
    /// <summary>
    /// Provides methods to send packets related to movers and moving entities.
    /// </summary>
    public interface IMoverPacketFactory
    {
        /// <summary>
        /// Sends an information telling that the entity moved.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="beginPosition">Starting position.</param>
        /// <param name="destinationPosition">Destination position.</param>
        /// <param name="angle">Entity angle.</param>
        /// <param name="state">Entity state.</param>
        /// <param name="stateFlag">Entity state flag.</param>
        /// <param name="motion">Entity motion.</param>
        /// <param name="motionEx">Entity motion ex.</param>
        /// <param name="loop">Loop.</param>
        /// <param name="motionOption">Motion options.</param>
        /// <param name="tickCount">Tick count.</param>
        void SendMoverMoved(IWorldEntity entity, Vector3 beginPosition, Vector3 destinationPosition,
            float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption,
            long tickCount);

        /// <summary>
        /// Sends a information telling that the entity has a different behavior.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="beginPosition">Starting position.</param>
        /// <param name="destinationPosition">Destination position.</param>
        /// <param name="angle">Entity angle.</param>
        /// <param name="state">Entity state.</param>
        /// <param name="stateFlag">Entity state flag.</param>
        /// <param name="motion">Entity motion.</param>
        /// <param name="motionEx">Entity motion ex.</param>
        /// <param name="loop">Loop.</param>
        /// <param name="motionOption">Motion options.</param>
        /// <param name="tickCount">Tick count.</param>
        void SendMoverBehavior(IWorldEntity entity, Vector3 beginPosition, Vector3 destinationPosition,
            float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption,
            long tickCount);

        /// <summary>
        /// Sends the entity destination position.
        /// </summary>
        /// <param name="movableEntity">Entity.</param>
        void SendDestinationPosition(IMovableEntity movableEntity);

        /// <summary>
        /// Sends the moving entity position.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void SendMoverPosition(IWorldEntity entity);

        /// <summary>
        /// Sends the entity angle.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="left"></param>
        void SendDestinationAngle(IWorldEntity entity, bool left);

        /// <summary>
        /// Sends the state mode to a specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="flags">StateMode flags.</param>
        /// <param name="item">Optional item.</param>
        void SendStateMode(IWorldEntity entity, StateModeBaseMotion flags, Item item = null);

        /// <summary>
        /// Sends the entity following a target.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="targetEntity">Target entity to follow.</param>
        /// <param name="distance">Distance.</param>
        void SendFollowTarget(IWorldEntity entity, IWorldEntity targetEntity, float distance);

        /// <summary>
        /// Sends the update of an attribute.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="attribute">Attribute to update.</param>
        /// <param name="newValue">New attribute value.</param>
        void SendUpdateAttributes(IWorldEntity entity, DefineAttributes attribute, int newValue);

        /// <summary>
        /// Sends a motion to the given entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="objectMessage">Motion message.</param>
        void SendMotion(IWorldEntity entity, ObjectMessageType objectMessage);
    }
}
