namespace Rhisis.World.Game.Components
{
    public class TimerComponent
    {
        public long NextMoveTime { get; set; }

        public long DespawnTime { get; set; }

        public long RespawnTime { get; set; }

        public long NextAttackTime { get; set; }

        public long LastAICheck { get; set; }

        public long NextHealTime { get; set; }

        public long LastSpeakTime { get; set; }
    }
}
