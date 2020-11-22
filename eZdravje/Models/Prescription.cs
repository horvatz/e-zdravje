using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.Models
{
    public class Prescription
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Free { get; set; }
        public bool IsUsed { get; set; }

#nullable enable
        public int? SpecialistId { get; set; }
        public Specialist? Specialist { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
