using ActivateRoles;
using Azure.Identity;
using CommandLine;
using Microsoft.Graph;
using Microsoft.Graph.Models;


Console.WriteLine("Hello, World!");


// Create an interactive authentication provider
var scopes = new[] { "RoleManagement.ReadWrite.Directory", "Directory.ReadWrite.All" };
var interactiveCredential = new InteractiveBrowserCredential();

// Create Graph client
var graphClient = new GraphServiceClient(interactiveCredential, scopes);

                // "/" means tenant-wide

Parser.Default.ParseArguments<Options>(args)
       .WithParsed<Options>(async o =>
       {

           // Create role activation request
           var request = new UnifiedRoleAssignmentScheduleRequest
           {
               Action = UnifiedRoleScheduleRequestActions.SelfActivate,
               PrincipalId = o.PrincipalId,
               RoleDefinitionId = o.RoleDefinitionId,
               DirectoryScopeId = o.DirectoryScopeId,
               Justification = o.Justification
           };

           if (o.Verbose)
           {
               Console.WriteLine("Role activation request details:");
               Console.WriteLine($"Principal ID: {o.PrincipalId}");
               Console.WriteLine($"Role Definition ID: {o.RoleDefinitionId}");
               Console.WriteLine($"Directory Scope ID: {o.DirectoryScopeId}");
               Console.WriteLine($"Justification: {o.Justification}");
           }

           try
           {
               var result = await graphClient.RoleManagement
                   .Directory
                   .RoleAssignmentScheduleRequests
                   .PostAsync(request);

               Console.WriteLine("Role activation request submitted successfully.");
               Console.WriteLine($"Request ID: {result?.Id}");
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error activating role: {ex.Message}");
           }
       });



    