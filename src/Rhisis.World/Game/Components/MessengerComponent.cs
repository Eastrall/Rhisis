using Rhisis.World.Game.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Game.Components
{
    public class MessengerComponent
    {
        /// <summary>
        /// Gets the list of friends in this <see cref="MessengerComponent"/>.
        /// </summary>
        public List<IPlayerEntity> Friends { get; }

        /// <summary>
        /// Returns a value indicating whether the specified memberId is a friend.
        /// </summary>
        /// <param name="memberId">The memberId to check against.</param>
        /// <returns></returns>
        public bool IsFriend(int memberId) => Friends.Any(x => x.Id == memberId);

        /// <summary>
        /// Creates a new <see cref="MessengerComponent"/> instance.
        /// </summary>
        public MessengerComponent()
        {
            Friends = new List<IPlayerEntity>();
        }
    }
}