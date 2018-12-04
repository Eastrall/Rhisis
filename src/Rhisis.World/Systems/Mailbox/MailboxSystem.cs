using NLog;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;

namespace Rhisis.World.Systems.Mailbox
{
    [System(SystemType.Notifiable)]
    public class MailboxSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity))
                return;

            if (!e.CheckArguments())
            {
                Logger.Error("Cannot execute mailbox action: {0} due to invalid arguments.", e.GetType());
                return;
            }

            switch(e)
            {
                case QueryMailboxEventArgs queryMailboxEvent:
                    QueryMailbox(playerEntity, queryMailboxEvent);
                    break;
            }
        }

        private void QueryMailbox(IPlayerEntity player, QueryMailboxEventArgs e)
        {
            // get mail
            WorldPacketFactory.SendMailbox(player);
        }

        /// <summary>
        /// Get mailbox via ISC when we don't have the data here
        /// </summary>
        private void SendMailboxRequest()
        {

        }

    }
}
