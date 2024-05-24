using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("GetAll"), Authorize(Roles = "Admin")]
        public async Task<APIResult> GetListUser()
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var users = _userService.GetAllUser();
            APIResult rs = new APIResult();
            return rs.Success(users);
        }

        [HttpPost("ModifyRole"), Authorize(Roles = "Admin")]
        public async Task<APIResult> ModifyRoleUser(UserRoleVM request)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.Body);
            var checkUserName = _userService.GetUserByID(request.UserID);
            if (checkUserName == null)
            {
                throw new BadRequestException("User not found", 404, 400);
            }          

            var user = _userService.ModifyUserRole(request.UserID, request.Role);

            APIResult rs = new APIResult();
            return rs.Success(user);
        }

        [HttpPost("ChangeStatus"), Authorize(Roles = "Admin")]
        public async Task<APIResult> ChangeUserStatus([BindRequired] int ID)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);

            var checkUser = _userService.GetUserByID(ID);
            if (checkUser == null)
            {
                throw new BadRequestException("User not found", 404, 400);
            }

            _userService.ChangeUserStatus(checkUser);

            APIResult rs = new APIResult();
            return rs.MessageSuccess("Changed user status success!");
        }
    }
}
