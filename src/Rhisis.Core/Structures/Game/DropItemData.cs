using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    [DataContract]
    public class DropItemData
    {
        [DataMember]
        public int ItemId { get; set; }

        [DataMember]
        public long Probability { get; set; }

        [DataMember]
        public int ItemMaxRefine { get; set; }

        [DataMember]
        public int Count { get; set; }
    }
}
