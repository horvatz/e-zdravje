using eZdravje.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.ViewModels
{
    public class SpecialistIndexData
    {
        public IEnumerable<Specialist> Specialists { get; set; }
        public IEnumerable<SpecialistCategory> SpecialistCategories { get; set; }
    
    }
}
