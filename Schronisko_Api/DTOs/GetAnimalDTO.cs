﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schronisko_Api.DTOs
{
    public class GetAnimalDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Photo { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public DetailVolunteerDTO VolunteerDTO { get; set; }
    }
}
