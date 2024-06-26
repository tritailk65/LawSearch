﻿using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlazorAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticalController : ControllerBase
    {
        private readonly IArticalService _articalService;
        private readonly ILogger<ArticalController> logger;

        public ArticalController(IArticalService articalService, ILogger<ArticalController> logger)
        {
            _articalService = articalService;
            this.logger = logger;
        }

        [HttpGet, Authorize]
        public APIResult GetAllArtical()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<Artical> dtResult = _articalService.GetAllArtical();
            APIResult rs = new APIResult();
            return rs.Success(dtResult);
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetArticalDetail([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            ArticalDetail dtResult = _articalService.GetArticalDetail(id);
            APIResult rs = new APIResult();
            return rs.Success(dtResult);
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetByLawID([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<Artical> dtResult = _articalService.GetListArticalByLawID(id);
            APIResult rs = new APIResult();
            return rs.Success(dtResult);
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public APIResult EditContentArtical([FromBody] Artical artical)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            _articalService.EditContentArtical(artical);
            APIResult rs = new();
            return rs.MessageSuccess("Chỉnh sửa nội dung Artical thành công!");
        }

    }
}
