using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Trade.EventArgs
{
    public class TradeRequestEventArgs : SystemEventArgs
    {
        public uint TargetId { get; }

        public TradeRequestEventArgs(uint targetId)
        {
            TargetId = targetId;
        }

        public override bool CheckArguments() => TargetId > 0;
    }
}
