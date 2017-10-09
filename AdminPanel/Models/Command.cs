using System.ComponentModel.DataAnnotations;


namespace AdminPanel.Models
{
    public class Command
    {
        public int CommandID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Controller { get; set; }

        [Required]
        [MaxLength(30)]
        public string Action { get; set; }

        [Required]
        [MaxLength(30)]
        public string CommandName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Command()
        {
          
        }
    }
}
