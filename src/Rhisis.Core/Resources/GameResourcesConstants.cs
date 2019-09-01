using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Game;
using System.IO;

namespace Rhisis.Core.Resources
{
    public class GameResourcesConstants
    {
        // Cache keys
        public const string Defines = "Defines";
        public const string Texts = "Texts";
        public const string Movers = nameof(MoverData);
        public const string Items = nameof(ItemData);
        public const string Dialogs = nameof(DialogLoader);
        public const string Shops = nameof(ShopLoader);
        public const string Jobs = nameof(JobData);
        public const string ExpTables = nameof(ExpTableLoader);
        public const string PenalityData = nameof(PenalityLoader);
        public const string Npcs = nameof(NpcLoader);

        public class Errors
        {
            public const string UnableLoadMapMessage = "Unable to load map '{0}'. Reason: {1}.";
            public const string UnableLoadMessage = "Unable to load {0}. Reason: {1}";
            public const string ObjectIgnoredMessage = "{0} id '{1}' was ignored. Reason: {2}.";
            public const string ObjectOverridedMessage = "{0} id '{1}' was overrided. Reason: {2}.";
            public const string ObjectErrorMessage = "{0} with id '{1}' has an error. Reason: {2}";
        }

        public class Paths
        {
            public static readonly string DataPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
            public static readonly string DialogsPath = Path.Combine(DataPath, "dialogs");
            public static readonly string ResourcePath = Path.Combine(DataPath, "res");
            public static readonly string MapsPath = Path.Combine(DataPath, "maps");
            public static readonly string ShopsPath = Path.Combine(DataPath, "shops");
            public static readonly string DataSub0Path = Path.Combine(ResourcePath, "data");
            public static readonly string DataSub1Path = Path.Combine(ResourcePath, "dataSub1");
            public static readonly string DataSub2Path = Path.Combine(ResourcePath, "dataSub2");
            public static readonly string MoversPropPath = Path.Combine(DataSub0Path, "propMover.txt");
            public static readonly string MoversPropExPath = Path.Combine(DataSub0Path, "propMoverEx.inc");
            public static readonly string ItemsPropPath = Path.Combine(DataSub2Path, "propItem.txt");
            public static readonly string WorldScriptPath = Path.Combine(DataSub0Path, "World.inc");
            public static readonly string JobPropPath = Path.Combine(DataSub1Path, "propJob.inc");
            public static readonly string TextClientPath = Path.Combine(DataSub1Path, "textClient.inc");
            public static readonly string ExpTablePath = Path.Combine(DataSub0Path, "expTable.inc");
            public static readonly string DeathPenalityPath = Path.Combine(ResourcePath, "deathPenality.json");
        }
    }
}
