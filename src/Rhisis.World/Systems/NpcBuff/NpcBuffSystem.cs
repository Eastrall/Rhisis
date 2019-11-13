using Microsoft.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Chat;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rhisis.World.Systems.NpcBuff
{
    [Injectable]
    public sealed class NpcBuffSystem : INpcBuffSystem
    {
        private readonly INpcBuffPacketFactory _npcBuffPacketFactory;

        public void NpcBuff(IPlayerEntity player, string buff)
        {
            /* */

        }
    }
}
