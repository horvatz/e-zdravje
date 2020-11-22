using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.Models
{
    public class SpecialistCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Specialist> Specialists { get; set; }
    }
}
