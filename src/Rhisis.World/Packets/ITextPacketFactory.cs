using Rhisis.Core.Data;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public interface ITextPacketFactory
    {
        /// <summary>
        /// Sends a defined text by its id to the current player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="textId">Defined text id.</param>
        void SendDefinedText(IPlayerEntity player, DefineText textId);

        /// <summary>
        /// Sends a defined text by its id with parameters to the current player.
        /// </summary>
        /// <param name="player">Current player.</param>
        /// <param name="textId">Defined text</param>
        /// <param name="parameters">Parameters.</param>
        void SendDefinedText(IPlayerEntity player, DefineText textId, params object[] parameters);

        /// <summary>
        /// Sends a system message to the player.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="text">System message text.</param>
        void SendSnoop(IPlayerEntity player, string text);

        /// <summary>
        /// Sends a message to the player telling that the feature is not implemented yet.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="feature">Feature name.</param>
        void SendFeatureNotImplemented(IPlayerEntity player, string feature);

        /// <summary>
        /// Sends a world message to a player.
        /// </summary>
        /// <param name="entity">Current player.</param>
        /// <param name="text">Text.</param>
        void SendWorldMessage(IPlayerEntity entity, string text);
    }
}
