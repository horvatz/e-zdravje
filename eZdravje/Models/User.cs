using Microsoft.AspNetCore.Identity;

namespace eZdravje.Models
{
    public class User : IdentityUser
    {
        public string UporabniskoIme { get; set; }
        public int Geslo { get; set; }
    }
}