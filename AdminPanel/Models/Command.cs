using System;
using System.ComponentModel.DataAnnotations;


namespace AdminPanel.Models 
{
    public class Command : IEquatable<Command>
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

        public bool Equals(Command other)
        {
            if (other == null)
            {
                return false;
            }

            return (Controller == other.Controller && Action == other.Action && CommandName == other.CommandName);
        }

        public override int GetHashCode()
        {
            return (Controller+Action+CommandName).GetHashCode();
        }
    }
}
