using System.ComponentModel.DataAnnotations;

namespace WebHangfire.Models
{public class CommandModel
    {
        [Required]
        public string? Command { get; set; }

        [Required]
        [Range(int.MinValue, int.MaxValue)]
        public int Parameter { get; set; }
    }
}
