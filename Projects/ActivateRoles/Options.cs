using CommandLine;

namespace ActivateRoles
{
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('p', "principalid", Required = true, HelpText = "Your Azure AD user object ID.")]
        public required string PrincipalId {get;set; }

        [Option('r', "roledefinitionid", Required = true, HelpText = "e.g., Global Administrator role")]
        public required string RoleDefinitionId { get; set; }

        [Option('d', "directoryscopeid", Required = false, HelpText = "Default to tenant-wide scope", Default ="/")]
        public string DirectoryScopeId { get; set; } = "/";
        // Replace with your info
        [Option('j', "justification", Required = false, HelpText = "Justification for activation", Default = "/")]
        public  string Justification { get; set; } = "Needed for maintenance";
    }
}
