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
                case AddFriendCancelEventArgs addFriendCancelEventArgs:
                    this.OnAddFriendCancel(playerEntity, addFriendCancelEventArgs);
                    break;

            }
        }

        private void OnAddFriendRequest(IPlayerEntity playerEntity, AddFriendRequestEventArgs e)
        {
            var member = playerEntity.Context.Entities
                .Where(x => x is IPlayerEntity memberEntity && memberEntity != null && memberEntity.PlayerData.Id == e.MemberId)
                .FirstOrDefault() as IPlayerEntity;

            if(member == null)
            {
                Logger.Error($"Player {e.MemberId} was not found.");
            }
            else
            {
                if(playerEntity.Messenger.IsFriend(member.Id))
                {
                    Logger.Warn($"Player {member.PlayerData.Id} is already a friend.");
                }
                else
                {
                    WorldPacketFactory.SendAddFriendRequest(playerEntity, member);
                }
            }
        }
        
        private void OnAddFriendCancel(IPlayerEntity playerEntity, AddFriendCancelEventArgs e)
        {
            var member = playerEntity.Context.Entities
                .Where(x => x is IPlayerEntity memberEntity && memberEntity != null && memberEntity.PlayerData.Id == e.LeaderId)
                .FirstOrDefault() as IPlayerEntity;

            Logger.Debug($"Player {playerEntity.PlayerData.Id} denied friend request of {e.LeaderId}");
            WorldPacketFactory.SendAddFriendCancel(playerEntity, member);
        }
    }
}