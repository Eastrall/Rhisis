using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Trade.EventArgs
{
    public class TradeRequestCancelEventArgs : SystemEventArgs
    {
        public uint TargetId { get; }

        public TradeRequestCancelEventArgs(uint targetId)
        {
            TargetId = targetId;
        }

        public override bool CheckArguments() => TargetId > 0;
    }
}
