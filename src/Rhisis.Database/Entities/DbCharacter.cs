using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rhisis.Database.Entities
{
    [Table("characters")]
    public sealed class DbCharacter : DbEntity
    {
        public int UserId { get; set; }

        public string Name { get; set; }
        
        public byte Gender { get; set; }
        
        public int Level { get; set; }
        
        public long Experience { get; set; }
        
        public int ClassId { get; set; }
        
        public int Gold { get; set; }
        
        public int Slot { get; set; }
        
        public int Strength { get; set; }
        
        public int Stamina { get; set; }
        
        public int Dexterity { get; set; }
        
        public int Intelligence { get; set; }
        
        public int Hp { get; set; }
        
        public int Mp { get; set; }
        
        public int Fp { get; set; }
        
        public int SkinSetId { get; set; }
        
        public int HairId { get; set; }

        public int HairColor { get; set; }
        
        public int FaceId { get; set; }
        
        public int MapId { get; set; }

        public int MapLayerId { get; set; }
        
        public float PosX { get; set; }
        
        public float PosY { get; set; }
        
        public float PosZ { get; set; }
        
        public float Angle { get; set; }
        
        public int BankCode { get; set; }
        
        public int StatPoints { get; set; }
        
        public int SkillPoints { get; set; }

        public DbUser User { get; set; }

        public ICollection<DbItem> Items { get; set; }

        public ICollection<DbMail> Mails { get; set; }
        
        public DbCharacter()
        {
            this.Items = new HashSet<DbItem>();
            this.Mails = new HashSet<DbMail>();
        }
    }
}
