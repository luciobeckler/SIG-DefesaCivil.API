using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIG_DefesaCivil.API.Models
{
    [Index(nameof(Token), IsUnique = true)]
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public string UserId { get; set; }

        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }

        [NotMapped]
        public bool IsActive => Revoked == null && DateTime.UtcNow <= Expires;
    }
}