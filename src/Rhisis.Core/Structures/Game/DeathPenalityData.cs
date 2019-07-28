using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    [DataContract]
    public class DeathPenalityData
    {
        /// <summary>
        /// Gets the revival penality data.
        /// </summary>
        [DataMember]
        public IEnumerable<PenalityData> RevivalPenality { get; set; }

        /// <summary>
        /// Gets or sets the Dec experience penality data.
        /// </summary>
        [DataMember]
        public IEnumerable<PenalityData> DecExpPenality { get; set; }

        /// <summary>
        /// Gets the level down penality data.
        /// </summary>
        [DataMember]
        public IEnumerable<PenalityData> LevelDownPenality { get; set; }

        /// <summary>
        /// Gets a revival penality by a level.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <returns>Revival penality expressed as a percentage.</returns>
        public decimal GetRevivalPenality(int level) => this.GetPenality(this.RevivalPenality, level).Value;

        /// <summary>
        /// Gets the experience penality by a level.
        /// </summary>
        /// <param name="level">Level.</param>
        /// <returns>Experience penality expressed as a percentage.</returns>
        public decimal GetDecExpPenality(int level) => this.GetPenality(this.DecExpPenality, level).Value;

        /// <summary>
        /// Gets the level down penality by a level.
        /// </summary>
        /// <param name="level">Level</param>
        /// <returns>Boolean value that indicates if level should go down.</returns>
        public bool GetLevelDownPenality(int level)
            => Convert.ToBoolean(this.GetPenality(this.LevelDownPenality, level).Value);

        /// <summary>
        /// Gets a <see cref="PenalityData"/> from a collection of penalities and a level.
        /// </summary>
        /// <param name="penalityData">Penality data colletion.</param>
        /// <param name="level">Level</param>
        /// <returns></returns>
        private PenalityData GetPenality(IEnumerable<PenalityData> penalityData, int level)
            => penalityData.FirstOrDefault(x => level <= x.Level) ?? penalityData.LastOrDefault();
    }
}
