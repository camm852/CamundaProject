using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CamundaSoftwareProject.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = string.Empty;
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("address")]
        public string Address { get; set;} = string.Empty;
        [Column("telephone")]
        public string Telephone { get; set; } = string.Empty;
        [Column("password")]
        public string Password { get; set; } = string.Empty; 
    }
}
