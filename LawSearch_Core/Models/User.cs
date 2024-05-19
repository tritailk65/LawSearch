using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }
        public string Email { get; set; }

        public User()
        {
            ID = -1;
            UserName = "DefaultUser";
            Password = "DefaultPassword";
            Type = -1;
            Email = "default@example.com";
        }
    }
}
