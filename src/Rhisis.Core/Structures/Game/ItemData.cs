using Rhisis.Core.Data;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    /// <summary>
    /// Represents an Item data from the propItem.txt file.
    /// </summary>
    [DataContract]
    public class ItemData
    {
        [DataMember(Name = "ver6")]
        public int Version { get; set; }

        [DataMember(Name = "dwID")]
        public int Id { get; set; }

        [DataMember(Name = "szName")]
        public string Name { get; set; }

        [DataMember(Name = "dwPackMax")]
        public int PackMax { get; set; }

        [DataMember(Name = "dwItemKind1")]
        public ItemKind1 ItemKind1 { get; set; }

        [DataMember(Name = "dwItemKind2")]
        public ItemKind2 ItemKind2 { get; set; }

        [DataMember(Name = "dwItemKind3")]
        public ItemKind3 ItemKind3 { get; set; }

        [DataMember(Name = "dwItemSex")]
        public int ItemSex { get; set; }

        [DataMember(Name = "dwCost")]
        public int Cost { get; set; }

        [DataMember(Name = "dwLimitLevel1")]
        public int LimitLevel { get; set; }

        [DataMember(Name = "dwParts")]
        public int Parts { get; set; }

        [DataMember(Name = "dwWeaponType")]
        public WeaponType WeaponType { get; set; }

        [DataMember(Name = "dwAbilityMin")]
        public int AbilityMin { get; set; }

        [DataMember(Name = "dwAbilityMax")]
        public int AbilityMax { get; set; }

        [DataMember(Name = "dwItemLV")]
        public int Level { get; set; }

        [DataMember(Name = "dwItemRare")]
        public int Rare { get; set; }

        [DataMember(Name = "dwAttackSpeed")]
        public float AttackSpeed { get; set; }

        [IgnoreDataMember]
        public bool IsStackable => this.PackMax > 1;
    }
}
