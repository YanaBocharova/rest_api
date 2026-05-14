using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    public class Account : BaseEntity
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
