using Microsoft.AspNetCore.Mvc;

using Hangfire;
namespace WebHangfire.Controllers
{
    public class SchedulerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Schedule(string command, string arguments, DateTime startTime, string recurrence, int? period)
        {
            // You can add validation and scheduling logic here
            // For demonstration, just log the values
            System.Diagnostics.Debug.WriteLine($"Scheduling: {command} {arguments} at {startTime} recurrence: {recurrence} period: {period}");
            
            // Example: Schedule with Hangfire (simple demo, not recurring)
            BackgroundJob.Schedule(() => ExecuteCommand(command, arguments), startTime - DateTime.Now);
            
            ViewBag.Message = "Command scheduled successfully!";
            return View("Index");
        }
        
        public void ExecuteCommand(string command, string arguments)
        {
            // Implement your command execution logic here
            System.Diagnostics.Debug.WriteLine($"Executing: {command} {arguments}");
        }
    }
}
