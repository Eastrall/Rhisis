using NLog;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Messenger.EventArgs;
using System.Linq;

namespace Rhisis.World.Systems.Messenger
{
    [System]
    public sealed class MessengerSystem : NotifiableSystemBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a new <see cref="MessengerSystem"/> instance.
        /// </summary>
        /// <param name="context"></param>
        public MessengerSystem(IContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        public override void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity playerEntity) || !e.CheckArguments())
            {
                Logger.Error("MessengerHandler: Invalid arguments");
                return;
            }

            switch (e)
            {
                case AddFriendRequestEventArgs addFriendRequestEventArgs:
                    this.OnAddFriendRequest(playerEntity, addFriendRequestEventArgs);
                    break;
            }
        }

        private void OnAddFriendRequest(IPlayerEntity entity, AddFriendRequestEventArgs e)
        {
            var member = entity.Context.Entities
                .Where(x => x is IPlayerEntity memberEntity && memberEntity != null && memberEntity.PlayerData.Id == e.MemberId)
                .FirstOrDefault() as IPlayerEntity;

            if(member == null)
            {
                Logger.Error($"Player {e.MemberId} was not found.");
            }
            else
            {
                if(entity.Messenger.IsFriend(member.Id))
                {
                    Logger.Warn($"Player {member.Id} is already a friend.");
                }
                else
                {
                    WorldPacketFactory.SendAddFriendRequest(entity, member);
                }
            }
        }
    }
}