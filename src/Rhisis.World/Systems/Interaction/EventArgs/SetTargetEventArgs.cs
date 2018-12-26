using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Interaction.EventArgs
{
    public class SetTargetEventArgs : SystemEventArgs
    {
        /// <summary>
        /// Gets the target id.
        /// </summary>
        public uint TargetId { get; }

        /// <summary>
        /// Gets a value indicating whether target should be cleared or not.
        /// </summary>
        public byte TargetingMode { get; }

        /// <summary>
        /// Creates a new <see cref="SetTargetEventArgs" /> instance.
        /// </summary>
        /// <param name="targetId">Target Id</param>
        /// <param name="bTargetingMode">Targeting Mode</param>
        public SetTargetEventArgs(uint targetId, byte bTargetingMode)
        {
            this.TargetId = targetId;
            this.TargetingMode = bTargetingMode;
        }

        /// <inheritdoc />
        public override bool CheckArguments()
        {
            return true;
        }
    }
}