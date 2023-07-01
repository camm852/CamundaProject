using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CamundaSoftwareProject.Models
{
    [Table("storage")]
    public class Storage
    {
        [Key]
        [Column("id")]
        public string Id {get; set;} = string.Empty;
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("color")]
        public string Color { get; set; } = string.Empty;
        [Column("amount")]
        public string Amount { get; set;} = string.Empty;


    }
}
