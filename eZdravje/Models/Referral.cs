using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.Models
{
    public class Referral
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public bool IsUsed { get; set; } = false;

#nullable enable
        public int? SpecialistId { get; set; }
        public Specialist? Specialist { get; set; }
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? SpecialistCategoryId { get; set; }
        public SpecialistCategory? SpecialistCategory { get; set; }
    }
}
