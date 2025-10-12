
using CommandLine;

namespace RaptorUtil
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
        [Option('p', "path", Required = true, HelpText = "Path for watching")]
        public string Path { get; set; } = string.Empty;
        [Option('f', "filter", Required = false, HelpText = "Extension filter for watching. (*.csv)")]
        public string Filter { get; set; } = "*";
    }
}
