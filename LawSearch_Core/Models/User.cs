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
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class UserVM
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserRoleVM
    {
        public int UserID { set; get; }
        public string Role { get; set; } = string.Empty;
    }

    public class UserInfoVM
    {
        public int ID { set; get; }
        public string Username { set; get; }
        public string Role { set; get; }
        public string Token { set; get; }
    }
}
