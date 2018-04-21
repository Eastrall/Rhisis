using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Say;

namespace Rhisis.World.Game.Chat
{
    public static class SayCommand
    {
        [ChatCommand("/say")]
        private static void SayCommandChat(IPlayerEntity player, int target, string[] parameters, SayEventArgs e)
        {
            Logger.Debug("{0} want to say", player.Object.Name);
            if (parameters.Length >= 1)
            {

                SaySystem.GetTargetSay(player, parameters, e);

                if (parameters.Length >= 2)
                {
                    SaySystem.PrivateMessageSay(player, parameters);
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