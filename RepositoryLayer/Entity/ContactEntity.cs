using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity
{
    public class ContactEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        [MaxLength(10)]
        [Required]
        public string PhoneNumber { get; set; }

        // Store only the Owner's ID
        [Required]
        public int OwnerId { get; set; }

        // Navigation property (optional, if you need to access UserEntity)
        [ForeignKey("OwnerId")]
        public UserEntity Owner { get; set; }
    }
}