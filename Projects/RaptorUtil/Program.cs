// See https://aka.ms/new-console-template for more information
using CommandLine;
using RaptorUtil;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Raptor")
        .LeftJustified()
        .Color(Color.Red));
Console.WriteLine("Monitor Path");




// Parse Options from command line arguments or use defaults
Parser.Default
    .ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
        {
            if (string.IsNullOrEmpty(o.Filter))
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = o.Path,
                    NotifyFilter = NotifyFilters.DirectoryName,
                    Filter = "*",
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                };

                watcher.Created += (sender, e) =>
                {
                    if (Directory.Exists(e.FullPath))
                    {
                        Console.WriteLine($"Target folder created: {e.FullPath}");
                        
                    }
                };
            }
            else
            {
                FileSystemWatcher csvWatcher = new FileSystemWatcher
                {
                    Path = o.Path,
                    NotifyFilter = NotifyFilters.FileName,
                    Filter = o.Filter,
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = Directory.Exists(o.Path) 
                };

                csvWatcher.Created += (sender, e) =>
                {
                    Console.WriteLine($"CSV file created: {e.FullPath}");
                };
            }
        });

Console.WriteLine("Press any key to exit.");
Console.ReadKey();



