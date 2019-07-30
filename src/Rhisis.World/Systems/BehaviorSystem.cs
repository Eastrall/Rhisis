using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Systems
{
    [System]
    public sealed class BehaviorSystem : ISystem
    {
        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player | WorldEntityType.Monster | WorldEntityType.Npc;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            if (!entity.Object.Spawned)
                return;

            switch (entity)
            {
                case IMonsterEntity monster:
                    monster.Behavior?.Update();
                    break;
                case INpcEntity npc:
                    npc.Behavior?.Update();
                    break;
                case IPlayerEntity player:
                    player.Behavior?.Update();
                    break;
            }
        }
    }
}
