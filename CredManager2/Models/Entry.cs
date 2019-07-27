using Postulate.Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CredManager2.Models
{
    [Identity(nameof(Id))]
    public class Entry
    {
        [PrimaryKey]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        [Required]
        public string Url { get; set; }

        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(50)]
        [Required]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;
        
        public int Id { get; set; }
    }
}
