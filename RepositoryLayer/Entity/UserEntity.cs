using System;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        public int Otp { get; set; } = 0;
    }
}