using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IUserService userService, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet, Authorize]
        [Route("Me")]
        public ActionResult<string> GetMe()
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var userName = _userService.GetMyName();
            return Ok(userName);
        }

        [HttpPost("AddUser"), Authorize(Roles = "Admin")]
        public async Task<APIResult> Register(UserVM request)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.Body);
            User user = new User();

            var checkUserName = _userService.GetUserName(request.Username);
            if (checkUserName != null)
            {
                throw new BadRequestException("Username already exists!",400,400);
            }

            //Có thể check trên FE 
            if( !IsPasswordValid(request.Password))
            {
                throw new BadRequestException("Password must be at least 6 characters long and include at least one uppercase letter and one special character.", 400, 400);
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = "User";
            user.Status = true;

            _userService.AddUserToList(user);
            APIResult rs = new();
            return rs.MessageSuccess("Register success!");
        }

        [HttpPost("Login")]
        public async Task<APIResult> Login(UserVM request)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.Body);
            var user = _userService.GetUserName(request.Username);
            if (user == null)
            {
                throw new BadRequestException("User not found.", 404, 400);
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new BadRequestException("Wrong password.", 400, 400);
            }

            //Acount not active
            if (!user.Status)
            {
                throw new BadRequestException("Your account is currently inactive. Please contact your administrator for assistance in reactivating your account.", 400, 400);
            }

            string token = CreateToken(user);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            UserInfoVM userInfo = new UserInfoVM
            {
                ID = user.ID,
                Username = user.Username,
                Role = user.Role,
                Token = token
            };

            APIResult rs = new APIResult();
            return rs.Success(userInfo);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private bool IsPasswordValid(string password)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[\W_]).{6,}$";
            return Regex.IsMatch(password, pattern);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
