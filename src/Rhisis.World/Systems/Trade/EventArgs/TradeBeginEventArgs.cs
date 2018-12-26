using Rhisis.World.Game.Core.Systems;

namespace Rhisis.World.Systems.Trade.EventArgs
{
    public class TradeBeginEventArgs : SystemEventArgs
    {
        public uint TargetId { get; }

        public TradeBeginEventArgs(uint targetId)
        {
            TargetId = targetId;
        }

        public override bool CheckArguments() => TargetId > 0;
    }
}
