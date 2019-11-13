using Rhisis.Core.Exceptions;
using Rhisis.Core.IO;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Interfaces;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Say;
using System;
using System.Linq.Expressions;

namespace Rhisis.World.Game.Chat
{
    public static class SayCommand
    {
        protected override Expression<Func<IEntity, bool>> Filter => x => x.Type == WorldEntityType.Player;
        [ChatCommand("/say")]
        private static void SayCommandChat(IPlayerEntity player, int target, string[] parameters, SayEventArgs e)
        {
            Logger.Debug("{0} want to say", args: player.Object.Name);
            if (parameters.Length >= 1)
            {
                string targetName = parameters[0];
                // verif si le parameters existe dans la liste des joueurs
                //verif si le parameters existe dans la liste des joueurs en ligne
                //utiliser le parameters en tant que pseudo de target
                var playerName = from p in player select targetName;


                if (e.TargetSayId == player.Id)
                {
                    throw new RhisisSystemException($"Can't say ourselve ({player.Object.Name})");
                }

                int targetSay = target.Id;

                if (parameters.Length >= 2)
                {
                    string privateMessage = string.Join(" ", parameters);

                    //faire que les parameters suivants soient un message entier // supprimer parameters[0]
                    WorldPacketFactory.SendSay(player, target, privateMessage);
                }  
            }
            else
            {
                Logger.Error("Chat: /say command must have at least one parameter.");
            }
        }
    }
}