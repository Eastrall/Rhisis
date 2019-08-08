using Rhisis.Core.Helpers;
using Rhisis.Core.IO;
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
    public class DefaultNpcBehavior : IBehavior
    {
        private const float OralTextRadius = 50f;

        private readonly INpcEntity _npc;

        public DefaultNpcBehavior(INpcEntity npcEntity)
        {
            this._npc = npcEntity;
        }

        /// <inheritdoc />
        public void Update()
        {
            this.UpdateOralText();
        }

        /// <inheritdoc />
        public virtual void OnArrived()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update NPC oral text.
        /// </summary>
        private void UpdateOralText()
        {
            if (this._npc.Data == null)
                return;

            if (this._npc.Timers.LastSpeakTime <= Time.TimeInSeconds())
            {
                if (this._npc.Data.HasDialog && !string.IsNullOrEmpty(this._npc.Data.Dialog.OralText))
                {
                    IEnumerable<IPlayerEntity> playersArount = from x in this._npc.Object.Entities
                                                               where x.Object.Position.IsInCircle(this._npc.Object.Position, OralTextRadius) &&
                                                               x is IPlayerEntity
                                                               select x as IPlayerEntity;

                    foreach (IPlayerEntity player in playersArount)
                    {
                        string text = this._npc.Data.Dialog.OralText.Replace(DialogVariables.PlayerNameText, player.Object.Name);

                        WorldPacketFactory.SendChatTo(this._npc, player, text);
                    }

                    this._npc.Timers.LastSpeakTime = Time.TimeInSeconds() + RandomHelper.Random(10, 15);
                }
            }
        }
    }
}
