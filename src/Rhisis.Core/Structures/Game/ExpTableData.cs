using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Core.Structures.Game
{
    public class ExpTableData
    {
        /// <summary>
        /// Gets the item drop luck data.
        /// </summary>
        public IEnumerable<long[]> ExpDropLuck { get; }

        /// <summary>
        /// Gets the Character experience table data.
        /// </summary>
        public IReadOnlyDictionary<int, CharacterExpTableData> CharacterExpTable { get; }

        /// <summary>
        /// Creates a new <see cref="ExpTableData"/> instance.
        /// </summary>
        /// <param name="expDropLuck">Experience drop luck data.</param>
        /// <param name="characterExpTable">Character experience table.</param>
        public ExpTableData(IEnumerable<long[]> expDropLuck, IReadOnlyDictionary<int, CharacterExpTableData> characterExpTable)
        {
            this.ExpDropLuck = expDropLuck;
            this.CharacterExpTable = characterExpTable;
        }

        /// <summary>
        /// Gets a drop luck by level and refine.
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="refine">Refine</param>
        /// <returns></returns>
        public long GetDropLuck(int level, int refine) => this.ExpDropLuck.ElementAt(level - 1).ElementAt(refine);

        /// <summary>
        /// Gets a character experience information based on a level.
        /// </summary>
        /// <param name="level">Character level.</param>
        /// <returns><see cref="CharacterExpTableData"/> matching the level.</returns>
        public CharacterExpTableData GetCharacterExp(int level) => this.CharacterExpTable.TryGetValue(level, out CharacterExpTableData value) ? value : default;
    }
}
