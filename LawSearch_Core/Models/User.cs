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
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [JsonPropertyName("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("tokenCreated")]
        public DateTime TokenCreated { get; set; }

        [JsonPropertyName("tokenExpires")]
        public DateTime TokenExpires { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        // Optional: Constructor for easier instantiation
        

        // Optional: Parameterless constructor for serialization/deserialization
        public User()
        {
        }
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
