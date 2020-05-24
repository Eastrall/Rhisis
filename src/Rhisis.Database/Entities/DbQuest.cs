using System;
using System.Diagnostics;

namespace Rhisis.Database.Entities
{
    [DebuggerDisplay("Id: {QuestId} | Finished: {Finished}")]
    public sealed class DbQuest : DbEntity
    {
        /// <summary>
        /// Gets or sets the quest id.
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the quest is finished.
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the quest is checked in the user interface.
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the quest is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the quest start date.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates that the patrol has been done.
        /// </summary>
        public bool IsPatrolDone { get; set; }

        /// <summary>
        /// Gets or sets the amount of monsters killed for the first monster type.
        /// </summary>
        public int MonsterKilled1 { get; set; }

        /// <summary>
        /// Gets or sets the amount of monsters killed for the second monster type.
        /// </summary>
        public int MonsterKilled2 { get; set; }

        /// <summary>
        /// Gets or sets the character id the quest belongs to.
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the character the quest belongs to.
        /// </summary>
        public DbCharacter Character { get; set; }
    }
}
