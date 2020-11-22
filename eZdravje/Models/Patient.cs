using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.Models
{
    public class Patient
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string Street { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
#nullable enable
        public int? SpecialistId { get; set; }
        public Specialist? Specialist { get; set; }
#nullable disable
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Referral> Referrals { get; set; }

    }
}
