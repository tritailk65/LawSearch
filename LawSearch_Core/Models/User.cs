using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public string Email { get; set; } = string.Empty;
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
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
