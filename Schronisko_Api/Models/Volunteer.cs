using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Schronisko_Api.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        public int Age { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }


    }
}