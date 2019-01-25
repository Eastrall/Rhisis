using Rhisis.World.Game.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class BattleComponent
    {
        /// <summary>
        /// Gets or sets the fighting target.
        /// </summary>
        public ILivingEntity Target { get; set; }

        /// <summary>
        /// Gets or sets multiple fighting targets.
        /// </summary>
        public IList<ILivingEntity> Targets { get; set; }

        /// <summary>
        /// Gets a value that indicates if the object is currently fighting.
        /// </summary>
        public bool IsFighting => this.Target != null || this.Targets.Any();

        /// <summary>
        /// Creates and initializes the <see cref="BattleComponent"/>.
        /// </summary>
        public BattleComponent()
        {
            this.Targets = new List<ILivingEntity>();
        }

        /// <summary>
        /// Reset the battle component to its initial state.
        /// </summary>
        public void Reset()
        {
            this.Target = null;
            this.Targets.Clear();
        }
    }
}
