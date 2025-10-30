using CommandLine;
using ConsoleApp3;
using Serilog;

Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

Parser.Default.ParseArguments<Options>(args)
       .WithParsed<Options>(o =>
       {
           if (o.Action.Equals("create", StringComparison.OrdinalIgnoreCase))
           {
               try
               {
                   Log.Information("Processing Files...");

                   var filenameTemplates = new List<string>
                   {
                       o.FileTemplate
                   };

                   var filenames = FileTaskManager.GenerateFilenames(filenameTemplates);
                   var listFileTasks = new List<FileTask>();
                   foreach (var fname in filenames)
                   {
                       var fileTask = new FileTask
                       {
                           FileId = listFileTasks.Count + 1,
                           Name = fname,
                           Path = $"/files/{fname}",
                           Size = 0,
                           Status = "Pending",
                           CreatedAt = DateTime.Now,
                           LastModifiedAt = DateTime.Now
                       };
                       listFileTasks.Add(fileTask);
                   }

                   FileTaskManager.ExportFileTasksToCsv(listFileTasks, o.FileOutputCsv);


                   // Find tasks created today whose status is not "Completed"
                   var today = DateTime.Today;
                   var toProcess = listFileTasks
                       .Where(t => t.CreatedAt.Date == today && !string.Equals(t.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                       .ToArray();

                   if (!toProcess.Any())
                   {
                       Log.Information("No files to process for today.");
                   }
                   else
                   {
                       foreach (var task in toProcess)
                       {
                           Log.Information("Processing {FileName} (Status: {Status})...", task.Name, task.Status ?? "null");
                           try
                           {
                               FileTaskManager.ProcessFile(task);
                               Log.Information("Processed {FileName}. New Status: {Status}, LastModifiedAt: {LastModifiedAt}, Comment: {Comment}",
                                   task.Name,
                                   task.Status,
                                   task.LastModifiedAt,
                                   task.Comment ?? "None");
                           }
                           catch (Exception exTask)
                           {
                               Log.Error(exTask, "Error processing {FileName}", task.Name);
                           }
                       }
                   }
               }
               catch (Exception ex)
               {
                   Log.Fatal(ex, "Unhandled exception in application");
                   throw;
               }
               finally
               {
                   Log.CloseAndFlush();
               }

           }
       });




