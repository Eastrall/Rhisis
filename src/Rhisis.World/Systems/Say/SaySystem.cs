using Rhisis.World.Core.Systems;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Interfaces;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems.Say
{
    [System]
    public class SaySystem : NotifiableSystemBase
    {
        private static readonly IDictionary<string, Action<IPlayerEntity, string[]>> ChatCommands = new Dictionary<string, Action<IPlayerEntity, string[]>>();
        protected override Expression<Func<IEntity, bool>> Filter => x => x.Type == WorldEntityType.Player;

        public SaySystem(IContext context)
    :   base(context)
        {
        }

        public static void GetTargetSay (IPlayerEntity player, int target, string[] parameters, SayEventArgs e)
        {
            // Grab character's name using parameters[0]

            var targetName = from p in player.Context.Entities select player.Id;
            // Get ID of player with his name + Check if it exist, if not, use RhisisSystemException
            // Check if the character is online to be able to send MP

            if (e.TargetSayId == player.Id)
            {
                throw new RhisisSystemException($"Can't say ourselve ({player.Object.Name})");
            }
        }
        public static void PrivateMessageSay(IPlayerEntity player, string[] parameters)
        {
            // Get all parameters[1+] and create un string to send as PM
            string privateMessage = string.Join(" ", parameters);
        }
    }
}
