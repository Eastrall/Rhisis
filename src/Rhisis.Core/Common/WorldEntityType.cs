using System;

namespace Rhisis.Core.Common
{
    /// <summary>
    /// Defines the different world entities.
    /// </summary>
    [Flags]
    public enum WorldEntityType
    {
        Player = 0x01,
        Monster = 0x02,
        Npc = 0x04,
        Pet = 0x08,
        Drop = 0x10,
        Mover = Player | Monster | Pet,
        Object = Player | Monster | Npc | Pet | Drop
    }
}
