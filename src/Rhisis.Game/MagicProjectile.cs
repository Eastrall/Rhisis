using Rhisis.Game.Abstractions;
using Rhisis.Game.Abstractions.Entities;
using Rhisis.Game.Common;
using System;
using System.Diagnostics;

namespace Rhisis.Game
{
    [DebuggerDisplay("Magic Projectile: {MagicPower}")]
    public class MagicProjectile : Projectile, IMagicProjectile
    {
        public override AttackFlags Type => AttackFlags.AF_MAGIC;

        public int MagicPower { get; }

        public MagicProjectile(IMover owner, IMover target, int magicPower, Action onArrived)
            : base(owner, target, onArrived)
        {
            MagicPower = magicPower;
        }
    }
}