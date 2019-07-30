namespace Rhisis.World.Game.Behaviors
{
    /// <summary>
    /// Provides a mechanism to manage an entity behavior.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Updates the AI/Behavior.
        /// </summary>
        void Update();

        /// <summary>
        /// Process an action when the entity arrives to its destination.
        /// </summary>
        void OnArrived();
    }
}
