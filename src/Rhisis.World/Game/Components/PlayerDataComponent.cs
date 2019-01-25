using Rhisis.Core.Common;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Game;

namespace Rhisis.World.Game.Components
{
    public class PlayerDataComponent
    {
        /// <summary>
        /// The version
        /// </summary>
        public const int StartVersion = 1;

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the player's slot.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Gets or sets the current amount of gold the player has.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the current shop name that the player is visiting.
        /// </summary>
        public string CurrentShopName { get; set; }

        /// <summary>
        /// Gets or sets the player's authority.
        /// </summary>
        public AuthorityType Authority { get; set; }

        /// <summary>
        /// Gets or sets the player's experience.
        /// </summary>
        public long Experience { get; set; }

        /// <summary>
        /// Gets or sets the Job Id.
        /// </summary>
        public int JobId
        {
            get => this._jobId;
            set
            {
                this._jobId = value;
                this.JobData = DependencyContainer.Instance.Resolve<JobLoader>().GetJob(this._jobId);
            }
        }
        private int _jobId;

        /// <summary>
        /// Gets the job's data.
        /// </summary>
        public JobData JobData { get; private set; }

        /// <summary>
        /// Gets the current version of the player data.
        /// Has to be updated (Version += 1) everytime one of these things changes: Job, Level, Gender, Online/Offline status
        /// </summary>
        public int Version { get; set; } = StartVersion;

        /// <summary>
        /// Gets or sets the player's mode.
        /// </summary>
        public ModeType Mode { get; set; }

        /// <summary>
        /// Creates a new <see cref="PlayerDataComponent"/> instance.
        /// </summary>
        public PlayerDataComponent()
        {
            this.Mode = ModeType.NONE;
        }
    }
}
