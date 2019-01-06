using Rhisis.Core.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    /// <summary>
    /// Represents a Mover data structure from the propMover.txt resource file from the data.res.
    /// </summary>
    [DataContract]
    public class MoverData
    {
        [DataMember(Name = "dwID")]
        public int Id { get; private set; }

        [DataMember(Name = "szName")]
        public string Name { get; set; }

        [DataMember(Name = "dwAI")]
        public int AI { get; set; }

        [DataMember(Name = "dwBelligerence")]
        public int Belligerence { get; set; }

        [DataMember(Name = "fSpeed")]
        public float Speed { get; set; }

        [DataMember(Name = "dwAddHp")]
        public int AddHp { get; set; }

        [DataMember(Name = "dwAddMp")]
        public int AddMp { get; set; }

        [DataMember(Name = "dwLevel")]
        public int Level { get; set; }

        [DataMember(Name = "dwFilghtLevel")]
        public int FlightLevel { get; set; }

        [DataMember(Name = "dwAtkMin")]
        public int AttackMin { get; set; }

        [DataMember(Name = "dwAtkMax")]
        public int AttackMax { get; set; }

        [DataMember(Name = "dwStr")]
        public int Strength { get; set; }

        [DataMember(Name = "dwSta")]
        public int Stamina { get; set; }

        [DataMember(Name = "dwDex")]
        public int Dexterity { get; set; }

        [DataMember(Name = "dwInt")]
        public int Intelligence { get; set; }

        [DataMember(Name = "dwHR")]
        public int HitRating { get; set; }

        [DataMember(Name = "dwER")]
        public int EscapeRating { get; set; }

        [DataMember(Name = "dwClass")]
        public MoverClassType Class { get; set; }

        [DataMember(Name = "dwNaturealArmor")]
        public int NaturalArmor { get; set; }

        [DataMember(Name = "dwResisMagic")]
        public int MagicResitance { get; set; }

        [DataMember(Name = "dwReAttackDelay")]
        public int ReAttackDelay { get; set; }

        [DataMember(Name = "dwAttackSpeed")]
        public int AttackSpeed { get; set; }

        [DataMember(Name = "dwCorrectionValue")]
        public int CorrectionValue { get; set; }

        [IgnoreDataMember]
        public int DropGoldMin { get; set; }

        [IgnoreDataMember]
        public int DropGoldMax { get; set; }

        [IgnoreDataMember]
        public int MaxDropItem { get; set; }

        [IgnoreDataMember]
        public ICollection<DropItemData> DropItems { get; }

        [IgnoreDataMember]
        public ICollection<DropItemKindData> DropItemsKind { get; }

        public MoverData()
        {
            this.DropItems = new List<DropItemData>();
            this.DropItemsKind = new List<DropItemKindData>();
        }

        public override string ToString() => this.Name;
    }
}
