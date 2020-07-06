using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schronisko_Api.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public int Age { get; set; }
        [MaxLength(50)]
        public string Reference { get; set; }        
        public int statusId { get; set; }
        public int volunteerId { get; set; }
        [MaxLength(200)]
        public string Photo { get; set; }
        [ForeignKey(nameof(statusId))]
        public virtual Status Status { get; set; }
        [ForeignKey(nameof(volunteerId))]
        public virtual Volunteer Volunteer { get; set; }
    }
}
