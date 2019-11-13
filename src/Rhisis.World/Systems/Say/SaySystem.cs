using Rhisis.Core.Exceptions;
using Rhisis.World.Core.Systems;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Interfaces;
using Rhisis.World.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        public static void GetTargetSay (IPlayerEntity player, string[] parameters, SayEventArgs e)
        {
            var targetName = from p in player. select parameters[0];

            if (e.TargetSayId == player.Id)
            {
                throw new RhisisSystemException($"Can't start a Trade with ourselve ({player.Object.Name})");
            }
        }

        public static void PrivateMessageSay(IPlayerEntity player, string[] parameters)
        {

        }
    }
}
