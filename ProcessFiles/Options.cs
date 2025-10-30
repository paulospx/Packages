using CommandLine;

namespace ConsoleApp3
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
        [Option('a', "action", Required = true, HelpText = "Action to perform: create, delete, update")]
        public string Action { get; set; } = "create";
        [Option('t', "teamples", Required = false, HelpText = "Template of the filename to create")]
        public string FileTemplate { get; set; } = "File-yyyyMMdd.txt";
        [Option('o', "output", Required = false, HelpText = "Output CSV file for file tasks")]
        public string FileOutputCsv { get; set; } = "FileTasks.csv";

    }
}
