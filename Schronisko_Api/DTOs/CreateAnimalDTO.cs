using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schronisko_Api.DTOs
{
    public class CreateAnimalDTO
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Reference { get; set; }
        public string Photo { get; set; }
        public string Status { get; set; }
        public ShortVolunteerDTO VolunteerDTO { get; set; }

    }
}
