using McMaster.Extensions.CommandLineUtils;

namespace Rhisis.CLI
{
    public static class Program
    {
        public const string Description = "This tool is a command line interface allowing user to manage their own servers easily.";

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => CommandLineApplication.Execute<Application>(args);
    }
}
