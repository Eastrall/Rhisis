using Rhisis.World.Game.Core;

namespace Rhisis.World.Systems.Say
{
    public class SayEventArgs : SystemEventArgs
    {
        public int TargetSayId { get; }

        public SayEventArgs(int targetSayId)
        {
            TargetSayId = targetSayId;
        }

        public override bool CheckArguments()
        {
            return TargetSayId != 0;
        }
    }
    public class SayEventArgsTwo : SystemEventArgs
    {
        public string PrivateMessage { get; }

        public SayEventArgsTwo(params object[] args)
            : base(args)
        {
            this.PrivateMessage = this.GetArgument<string>(0);
        }

        public override bool CheckArguments() => true;
    }
}