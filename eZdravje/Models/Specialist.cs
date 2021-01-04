using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eZdravje.Models
{
    
    public class Specialist
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public string Street { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }

#nullable enable
        public int? SpecialistCategoryId { get; set; }
        public SpecialistCategory? SpecialistCategory { get; set; }
        public string? UsersId { get; set; }
#nullable disable
        public virtual ICollection<Patient> Patients { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Referral> Referrals { get; set; }
    }

}


