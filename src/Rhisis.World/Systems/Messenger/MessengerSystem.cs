using NLog;
using Rhisis.Core.Data;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Messenger.EventArgs;
using System;
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
                case AddFriendNameRequestEventArgs addFriendNameRequestEventArgs:
                    this.OnAddFriendNameRequest(playerEntity, addFriendNameRequestEventArgs);
                    break;
                case AddFriendEventArgs addFriendEventArgs:
                    this.OnAddFriend(playerEntity, addFriendEventArgs);
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

            OnAddFriendRequest(playerEntity, member);
        }

        private void OnAddFriendNameRequest(IPlayerEntity playerEntity, AddFriendNameRequestEventArgs e)
        {
            var member = playerEntity.Context.Entities
                .Where(x => x is IPlayerEntity memberEntity && string.Equals(memberEntity.Object.Name, e.MemberName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault() as IPlayerEntity;

            OnAddFriendRequest(playerEntity, member);
        }

        private void OnAddFriendRequest(IPlayerEntity playerEntity, IPlayerEntity memberEntity)
        {
            if (memberEntity == null)
            {
                Logger.Error($"Player {memberEntity.PlayerData.Id} was not found.");
            }
            else
            {
                if (playerEntity.Messenger.IsFriend(memberEntity.Id))
                {
                    Logger.Warn($"Player {memberEntity.PlayerData.Id} is already a friend.");
                }
                else
                {
                    WorldPacketFactory.SendAddFriendRequest(playerEntity, memberEntity);
                }
            }
        }

        private void OnAddFriend(IPlayerEntity playerEntity, AddFriendEventArgs e)
        {
            var friend = playerEntity.Context.Entities
                .Where(x => x is IPlayerEntity memberEntity && memberEntity != null && memberEntity.PlayerData.Id == e.MemberId)
                .FirstOrDefault() as IPlayerEntity;

            if (friend == null)
            {
                Logger.Warn($"Player {e.MemberId} does not exist.");
            }
            else
            {
                Logger.Debug($"Adding each other to friend list.");

                friend.Messenger.Friends.Add(playerEntity);
                playerEntity.Messenger.Friends.Add(friend);

                // TODO: Check Max Friends

                WorldPacketFactory.SendAddFriend(friend, playerEntity);
                WorldPacketFactory.SendAddFriend(playerEntity, friend);

                // TODO: SendDefinedText with name parameter.
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