namespace Rhisis.Database.Entities
{
    public class DbSkill : DbEntity
    {
        /// <summary>
        /// Gets or sets the skill id.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Gets or sets the skill level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the character id.
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        public DbCharacter Character { get; set; }
    }
}
