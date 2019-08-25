using Ether.Network.Packets;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
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
        private readonly ILogger<PlayerHandler> _logger;
        private readonly ISpecialEffectSystem _specialEffectSystem;
        private readonly IInteractionSystem _interationSystem;
        private readonly IFollowSystem _followSystem;
        private readonly IMoverPacketFactory _moverPacketFactory;
        private readonly IDeathSystem _deathSystem;

        public PlayerHandler(ILogger<PlayerHandler> logger, ISpecialEffectSystem specialEffectSystem, IInteractionSystem interationSystem, IFollowSystem followSystem, IMoverPacketFactory moverPacketFactory, IDeathSystem deathSystem)
        {
            this._logger = logger;
            this._specialEffectSystem = specialEffectSystem;
            this._interationSystem = interationSystem;
            this._followSystem = followSystem;
            this._moverPacketFactory = moverPacketFactory;
            this._deathSystem = deathSystem;
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

        //[HandlerAction(PacketType.QUERY_PLAYER_DATA)]
        public void OnQueryPlayerData(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerDataPacket = new QueryPlayerDataPacket(packet);
            var queryPlayerDataEvent = new QueryPlayerDataEventArgs(onQueryPlayerDataPacket.PlayerId, onQueryPlayerDataPacket.Version);
            SystemManager.Instance.Execute<PlayerDataSystemOld>(client.Player, queryPlayerDataEvent);
        }

        //[HandlerAction(PacketType.QUERY_PLAYER_DATA2)]
        public void OnQueryPlayerData2(WorldClient client, INetPacketStream packet)
        {
            var onQueryPlayerData2Packet = new QueryPlayerData2Packet(packet);
            var queryPlayerData2Event = new QueryPlayerData2EventArgs(onQueryPlayerData2Packet.Size, onQueryPlayerData2Packet.PlayerDictionary);
            SystemManager.Instance.Execute<PlayerDataSystemOld>(client.Player, queryPlayerData2Event);
        }

        [HandlerAction(PacketType.PLAYERMOVED)]
        public void OnPlayerMoved(WorldClient client, PlayerMovedPacket packet)
        {
            if (client.Player.Health.IsDead)
            {
                this._logger.LogError($"Player {client.Player.Object.Name} is dead, he cannot move with keyboard.");
                return;
            }

            // TODO: Check if player is flying

            client.Player.Follow.Reset();
            client.Player.Battle.Reset();
            client.Player.Object.Position = packet.BeginPosition + packet.DestinationPosition;
            client.Player.Object.Angle = packet.Angle;
            client.Player.Object.MovingFlags = (ObjectState)packet.State;
            client.Player.Object.MotionFlags = (StateFlags)packet.StateFlag;
            client.Player.Moves.IsMovingWithKeyboard = client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_FMOVE) || 
                client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE);
            client.Player.Moves.DestinationPosition = packet.BeginPosition + packet.DestinationPosition;

            this._moverPacketFactory.SendMoverMoved(client.Player,
                packet.BeginPosition,
                packet.DestinationPosition,
                client.Player.Object.Angle, 
                (uint)client.Player.Object.MovingFlags, 
                (uint)client.Player.Object.MotionFlags,
                packet.Motion,
                packet.MotionEx,
                packet.Loop,
                packet.MotionOption,
                packet.TickCount);
        }

        [HandlerAction(PacketType.PLAYERBEHAVIOR)]
        public void OnPlayerBehavior(WorldClient client, PlayerBehaviorPacket packet)
        {
            if (client.Player.Health.IsDead)
            {
                this._logger.LogError($"Player {client.Player.Object.Name} is dead, he cannot move with keyboard.");
                return;
            }

            // TODO: check if player is flying

            client.Player.Object.Position = packet.BeginPosition + packet.DestinationPosition;
            client.Player.Object.Angle = packet.Angle;
            client.Player.Object.MovingFlags = (ObjectState)packet.State;
            client.Player.Object.MotionFlags = (StateFlags)packet.StateFlag;
            client.Player.Moves.IsMovingWithKeyboard = client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_FMOVE) ||
                client.Player.Object.MovingFlags.HasFlag(ObjectState.OBJSTA_BMOVE);
            client.Player.Moves.DestinationPosition = packet.BeginPosition + packet.DestinationPosition;

            this._moverPacketFactory.SendMoverBehavior(client.Player,
                packet.BeginPosition,
                packet.DestinationPosition,
                client.Player.Object.Angle,
                (uint)client.Player.Object.MovingFlags,
                (uint)client.Player.Object.MotionFlags,
                packet.Motion,
                packet.MotionEx,
                packet.Loop,
                packet.MotionOption,
                packet.TickCount);
        }

        [HandlerAction(PacketType.REVIVAL_TO_LODESTAR)]
        public void OnRevivalToLodestar(WorldClient client, INetPacketStream _)
        {
            if (!client.Player.Health.IsDead)
            {
                this._logger.LogWarning($"Player '{client.Player.Object.Name}' tried to revival to lodestar without being dead.");
                return;
            }

            this._deathSystem.ResurectLodelight(client.Player);
        }
    }
}
