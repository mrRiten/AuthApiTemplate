using System.ComponentModel.DataAnnotations;

namespace AuthApiTemplate.Entity
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required]
        public required string HashPassword { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required int RoleId { get; set; }

        [Required]
        public required string ConfirmToken { get; set; }

        public string? RefreshToken { get; set; }

        public Role? Role { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
