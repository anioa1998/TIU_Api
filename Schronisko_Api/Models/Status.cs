using System.ComponentModel.DataAnnotations;

namespace Schronisko_Api.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }

    }
}
