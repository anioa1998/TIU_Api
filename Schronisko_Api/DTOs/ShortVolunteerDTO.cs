using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schronisko_Api.DTOs
{
    public class ShortVolunteerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
