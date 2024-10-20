using System.ComponentModel.DataAnnotations;

namespace AuthApiTemplate.Entity
{
    public class Role
    {
        [Key]
        public int IdRole { get; set; }

        [Required]
        public required string RoleName { get; set; }
    }
}
