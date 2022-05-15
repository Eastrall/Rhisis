﻿using Rhisis.Core.Helpers;
using Rhisis.Abstractions;
using Rhisis.Abstractions.Entities;
using Rhisis.Game.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Rhisis.Abstractions.Protocol;

namespace Rhisis.Game
{
    public class Buff : IBuff
    {
        public uint Id { get; }

        public virtual BuffType Type { get; }

        public IMover Owner { get; }

        public int RemainingTime { get; set; }

        public bool HasExpired => RemainingTime <= 0;

        public IReadOnlyDictionary<DefineAttributes, int> Attributes { get; }

        public Buff(IMover owner, IDictionary<DefineAttributes, int> attributes)
        {
            Id = RandomHelper.GenerateUniqueId();
            Owner = owner;
            Attributes = new Dictionary<DefineAttributes, int>(attributes);
        }

        public void DecreaseTime(int time = 1)
        {
            RemainingTime -= time * 1000;
        }

        public bool Equals([AllowNull] IBuff other) => Id == other?.Id;

        public virtual void Serialize(IFFPacket packet)
        {
            // Nothing to do.
        }
    }
}