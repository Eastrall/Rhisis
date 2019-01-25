using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
using Rhisis.Core.Structures.Game;
using Rhisis.Core.Structures.Game.Dialogs;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Default behavior of a NPC.
    /// </summary>
    [Behavior(BehaviorType.Npc, IsDefault: true)]
    public class DefaultNpcBehavior : IBehavior<INpcEntity>
    {
        private static readonly float OralTextRadius = 50f;

        public void Update(INpcEntity entity)
        {
            this.UpdateOralText(entity);
        }

        /// <inheritdoc />
        public void OnArrived(INpcEntity entity) => throw new NotImplementedException();

        /// <summary>
        /// Update NPC oral text.
        /// </summary>
        /// <param name="npc">NPC Entity</param>
        private void UpdateOralText(INpcEntity npc)
        {
            if (npc.Timers.LastSpeakTime <= Time.TimeInSeconds())
            {
                if (npc.Data != null && npc.Data.HasDialog && !string.IsNullOrEmpty(npc.Data.Dialog.OralText))
                {
                    IEnumerable<IPlayerEntity> playersArount = from x in npc.Object.Entities
                                                               where x.Object.Position.IsInCircle(npc.Object.Position, OralTextRadius) &&
                                                               x is IPlayerEntity
                                                               select x as IPlayerEntity;

                    foreach (IPlayerEntity player in playersArount)
                    {
                        string text = npc.Data.Dialog.OralText.Replace(DialogVariables.PlayerNameText, player.Object.Name);

                        WorldPacketFactory.SendChatTo(npc, player, text);
                    }

                    npc.Timers.LastSpeakTime = Time.TimeInSeconds() + RandomHelper.Random(10, 15);
                }
            }
        }
    }
}
