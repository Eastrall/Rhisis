using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.Network.Packets.World;
using Rhisis.World.Client;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Death;
using Rhisis.World.Systems.Follow;
using Rhisis.World.Systems.Interaction;
using Rhisis.World.Systems.PlayerData;
using Rhisis.World.Systems.PlayerData.EventArgs;
using Rhisis.World.Systems.SpecialEffect;
using Sylver.HandlerInvoker.Attributes;
using System;

namespace Rhisis.World.Handlers
{
    [Handler]
    public sealed class PlayerHandler
    {
        private static readonly ILogger Logger = DependencyContainer.Instance.Resolve<ILogger<PlayerHandler>>();
        private readonly ILogger<PlayerHandler> _logger;
        private readonly ISpecialEffectSystem _specialEffectSystem;
        private readonly IInteractionSystem _interationSystem;
        private readonly IFollowSystem _followSystem;

        public PlayerHandler(ILogger<PlayerHandler> logger, ISpecialEffectSystem specialEffectSystem, IInteractionSystem interationSystem, IFollowSystem followSystem)
        {
            this._logger = logger;
            this._specialEffectSystem = specialEffectSystem;
            this._interationSystem = interationSystem;
            this._followSystem = followSystem;
        }

        [HandlerAction(PacketType.STATEMODE)]
        public void OnStateMode(IWorldClient client, StateModePacket packet)
        {
            if (client.Player.Object.StateMode == packet.StateMode)
            {
                if (packet.Flag == StateModeBaseMotion.BASEMOTION_CANCEL)
                {
                    this._specialEffectSystem.SetStateModeBaseMotion(client.Player, packet.Flag);
                    client.Player.Delayer.CancelAction(client.Player.Inventory.ItemInUseActionId);
                    client.Player.Inventory.ItemInUseActionId = Guid.Empty;
                }
            }
        }

        [HandlerAction(PacketType.SETTARGET)]
        public void OnSetTarget(IWorldClient client, SetTargetPacket packet)
        {
            this._interationSystem.SetTarget(client.Player, packet.TargetId, packet.TargetMode);
        }

        [HandlerAction(PacketType.PLAYERSETDESTOBJ)]
        public void OnPlayerSetDestObject(WorldClient client, PlayerDestObjectPacket packet)
        {
            this._followSystem.Follow(client.Player, packet.TargetObjectId, packet.Distance);
        }

        [PacketHandler(PacketType.QUERY_PLAYER_DATA)]
        public static void OnQueryPlayerData(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerDataPacket = new QueryPlayerDataPacket(packet);
            var queryPlayerDataEvent = new QueryPlayerDataEventArgs(onQueryPlayerDataPacket.PlayerId, onQueryPlayerDataPacket.Version);
            SystemManager.Instance.Execute<PlayerDataSystemOld>(client.Player, queryPlayerDataEvent);
        }

        [PacketHandler(PacketType.QUERY_PLAYER_DATA2)]
        public static void OnQueryPlayerData2(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerData2Packet = new QueryPlayerData2Packet(packet);
            var queryPlayerData2Event = new QueryPlayerData2EventArgs(onQueryPlayerData2Packet.Size, onQueryPlayerData2Packet.PlayerDictionary);
            SystemManager.Instance.Execute<PlayerDataSystemOld>(client.Player, queryPlayerData2Event);
        }

        [PacketHandler(PacketType.PLAYERMOVED)]
        public static void OnPlayerMoved(WorldClient client, INetPacketStream packet)
        {
            var playerMovedPacket = new PlayerMovedPacket(packet);

            if (client.Player.Health.IsDead)
            {
                Logger.LogError($"Player {client.Player.Object.Name} is dead, he cannot move with keyboard.");
                return;
            }

            // TODO: Check if player is flying

            client.Player.Follow.Reset();
            client.Player.Battle.Reset();
            client.Player.Object.Position = playerMovedPacket.BeginPosition + playerMovedPacket.DestinationPosition;
            client.Player.Object.Angle = playerMovedPacket.Angle;
            client.Player.Object.MovingFlags = (ObjectState)playerMovedPacket.State;
            client.Player.Object.MotionFlags = (StateFlags)playerMovedPacket.StateFlag;
            client.Player.Moves.IsMovingWithKeyboard = client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_FMOVE) || 
                client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE);
            client.Player.Moves.DestinationPosition = playerMovedPacket.BeginPosition + playerMovedPacket.DestinationPosition;

            WorldPacketFactory.SendMoverMoved(client.Player,
                playerMovedPacket.BeginPosition, 
                playerMovedPacket.DestinationPosition,
                client.Player.Object.Angle, 
                (uint)client.Player.Object.MovingFlags, 
                (uint)client.Player.Object.MotionFlags, 
                playerMovedPacket.Motion, 
                playerMovedPacket.MotionEx, 
                playerMovedPacket.Loop, 
                playerMovedPacket.MotionOption, 
                playerMovedPacket.TickCount);
        }

        [PacketHandler(PacketType.PLAYERBEHAVIOR)]
        public static void OnPlayerBehavior(WorldClient client, INetPacketStream packet)
        {
            var playerBehaviorPacket = new PlayerBehaviorPacket(packet);

            if (client.Player.Health.IsDead)
            {
                Logger.LogError($"Player {client.Player.Object.Name} is dead, he cannot move with keyboard.");
                return;
            }

            // TODO: check if player is flying

            client.Player.Object.Position = playerBehaviorPacket.BeginPosition + playerBehaviorPacket.DestinationPosition;
            client.Player.Object.Angle = playerBehaviorPacket.Angle;
            client.Player.Object.MovingFlags = (ObjectState)playerBehaviorPacket.State;
            client.Player.Object.MotionFlags = (StateFlags)playerBehaviorPacket.StateFlag;
            client.Player.Moves.IsMovingWithKeyboard = client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_FMOVE) ||
                client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE);
            client.Player.Moves.DestinationPosition = playerBehaviorPacket.BeginPosition + playerBehaviorPacket.DestinationPosition;

            WorldPacketFactory.SendMoverBehavior(client.Player,
                playerBehaviorPacket.BeginPosition,
                playerBehaviorPacket.DestinationPosition,
                client.Player.Object.Angle,
                (uint)client.Player.Object.MovingFlags,
                (uint)client.Player.Object.MotionFlags,
                playerBehaviorPacket.Motion,
                playerBehaviorPacket.MotionEx,
                playerBehaviorPacket.Loop,
                playerBehaviorPacket.MotionOption,
                playerBehaviorPacket.TickCount);
        }

        [PacketHandler(PacketType.REVIVAL_TO_LODESTAR)]
        public static void OnRevivalToLodestar(WorldClient client, INetPacketStream _)
        {
            if (!client.Player.Health.IsDead)
            {
                Logger.LogWarning($"Player '{client.Player.Object.Name}' tried to revival to lodestar without being dead.");
                return;
            }

            SystemManager.Instance.Execute<DeathSystem>(client.Player, null);
        }
    }
}
