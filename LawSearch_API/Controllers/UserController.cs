using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BlazorAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService _userService, ILogger<UserController> _logger)
        {
            userService = _userService;
            logger = _logger;
        }

        [HttpGet("[action]")]
        public APIResult GetUserByID([BindRequired] int id)
        {
            APIResult rs = new APIResult();
            var user = userService.GetUserByID(id);
            if (user == null || user.ID == -1)
            {
                return rs.MessageSuccess("User not found");
            }
            return rs.Success(user);
        }

        [HttpPost("[action]")]
        public APIResult CreateNewUser([FromBody] User user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return new APIResult()
                {
                    Status = 400,
                    Message = "Invalid user data",
                    Data = ModelState
                };
            }

            try
            {
                var newUser = userService.CreateNewUser(user);
                return new APIResult().Success(newUser);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating user");
                return new APIResult()
                {
                    Status = 500,
                    Message = "Internal server error",
                    Exception = ex.Message
                };
            }
        }
    }
}
