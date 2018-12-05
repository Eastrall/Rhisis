using NLog;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Mailbox.EventArgs;

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
                    GetMails(playerEntity, queryMailboxEvent);
                    break;
                case QueryPostMailEventArgs queryPostMailEvent:
                    SendMail(playerEntity, queryPostMailEvent);
                    break;
                case QueryRemoveMailEventArgs queryRemoveMailEvent:
                    RemoveMail(playerEntity, queryRemoveMailEvent);
                    break;
                case QueryGetMailItemEventArgs queryGetMailItemEvent:
                    GetMailItem(playerEntity, queryGetMailItemEvent);
                    break;
                case QueryGetMailGoldEventArgs queryGetMailGoldEvent:
                    GetMailMoney(playerEntity, queryGetMailGoldEvent);
                    break;
                case ReadMailEventArgs readMailEvent:
                    ReadMail(playerEntity, readMailEvent);
                    break;
            }
        }

        private void GetMails(IPlayerEntity player, QueryMailboxEventArgs e)
        {
            WorldPacketFactory.SendMailbox(player);
        }

        private void SendMail(IPlayerEntity player, QueryPostMailEventArgs e)
        {
            
        }

        private void RemoveMail(IPlayerEntity player, QueryRemoveMailEventArgs e)
        {
            // Delete mail, no packet back needed
        }

        private void GetMailItem(IPlayerEntity player, QueryGetMailItemEventArgs e)
        {
            // Remove item from mail, probably send got item packet back
        }

        private void GetMailMoney(IPlayerEntity player, QueryGetMailGoldEventArgs e)
        {
            // Remove money from mail, probably send got money packet back
        }

        private void ReadMail(IPlayerEntity player, ReadMailEventArgs e)
        {
            // Set mail to read, no packet back needed
        }
    }
}
