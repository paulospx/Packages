using ActivateRoles;
using Azure.Identity;
using CommandLine;
using Microsoft.Graph;
using Microsoft.Graph.Models;

// Entry point message
Console.WriteLine("Activate Azure Roles");

/// <summary>
/// Selects a random item from a list of strings.
/// Returns an empty string if the list is null or empty.
/// </summary>
/// <param name="items">List of strings to select from.</param>
/// <returns>Randomly selected string or empty string.</returns>
static string SelectItemFromList(List<string> items)
{
    if (items == null || items.Count == 0)
    {
        return string.Empty;
    }

    // Select random item from list items
    Random rand = new Random();
    int selection = rand.Next(1, items.Count + 1); // +1 because upper bound is exclusive
    {
        return items[selection - 1];
    }
}


// List of humorous reasons for role activation, printed randomly
var reasons = new List<string>
{
    "Because Azure won’t even let you open a Word doc until you’ve proven you’re the Chosen One.",
    "Without roles, every button in the portal just redirects to Microsoft Solitaire.",
    "Your manager insists it’s for “security,” but really, it’s just to stop you from accidentally spinning up 47 Kubernetes clusters. Again.",
    "Azure has separation anxiety and needs you to “activate” before it trusts you with anything more than renaming a resource group.",
    "If you don’t, the entire portal runs in demo mode where all VMs are actually just IKEA meatball recipes.",
    "Because Azure AD admins get bored and like to watch you fumble through the 7-step approval process like it’s Squid Game.",
    "Without roles, all your PowerShell scripts return nothing but dad jokes in Base64.",
    "The roles are basically Infinity Stones — without them, you’re just Bruce Banner trying to click around.",
    "You want to delete that resource group? Too bad. Azure just emailed your grandma to ask for MFA approval.",
    "Because 'least privilege' is just Microsoft’s polite way of saying “we don’t trust you.”",
    "If you skip role activation, Azure automatically spins up a 16-core VM running Minesweeper and bills your cost center $12,000.",
    "Azure wants to feel important. If you don’t activate a role, it sulks and gives you random 403 errors.",
    "Without roles, you can see the VM, but only as a decorative hologram.",
    "If you forget, the Resource Manager deploys passive-aggressive sticky notes instead of servers.",
    "Azure Security Center gets lonely and just starts spamming you with fake “critical alerts” until you cave.",
    "Without roles, you can only deploy resources into the mysterious “NotAuthorized” region.",
    "If you don’t activate, the Azure portal switches to Clippy mode: “It looks like you’re trying to work… do you want help activating a role?”",
    "Because watching the approval workflow slowly grind through three managers is the true Azure experience.",
    "Without it, your DevOps pipeline runs successfully… but only deploys a single JPEG of Satya Nadella smiling.",
    "Let’s be honest: activating roles isn’t about security — it’s just Microsoft’s way of ensuring you have time to go get coffee."
};

// Print a random reason for role activation
Console.WriteLine(SelectItemFromList(reasons));

// Define required Microsoft Graph scopes for role management
var scopes = new[] { "RoleManagement.ReadWrite.Directory", "Directory.ReadWrite.All" };

// Create an interactive authentication provider for Microsoft Graph
var interactiveCredential = new InteractiveBrowserCredential();

// Create Graph client using the interactive credential and scopes
var graphClient = new GraphServiceClient(interactiveCredential, scopes);
// "/" means tenant-wide

// Parse command-line arguments and handle role activation
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

           // Verbose output for debugging and auditing
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
               // Submit the role activation request to Microsoft Graph
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


