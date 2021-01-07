using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eZdravje.Models
{
    public class ActivationCode
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Role { get; set; }

        public string UserId { get; set; }
        public bool IsUsed { get; set; }
    }
}
