using Rhisis.Core.Structures;

namespace Rhisis.World.Game.Components
{
    public class MovableComponent
    {
        public long MoveTime { get; set; }

        public long LastMoveTime { get; set; }
        
        public long NextMoveTime { get; set; }


        public Vector3 DestinationPosition { get; set; }

        public bool HasArrived => this.DestinationPosition.IsZero();

        public float Speed { get; set; }

        public float SpeedFactor { get; set; }

        public Vector3 BeginPosition { get; set; }

        public bool ReturningToOriginalPosition { get; set; }

        public MovableComponent()
        {
            this.DestinationPosition = new Vector3();
            this.BeginPosition = new Vector3();
            this.SpeedFactor = 1f;
        }
    }
}
