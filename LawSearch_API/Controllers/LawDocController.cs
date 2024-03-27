using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LawDocController : ControllerBase
    {
        private readonly ILawDocService lawDocService;
        private readonly ILogger<LawDocController> logger;

        public LawDocController(ILawDocService lawDocService, ILogger<LawDocController> logger)
        {
            this.lawDocService = lawDocService;
            this.logger = logger;
        }

        // GET: api/<LawDocController>
        [HttpGet]
        public APIResult GetListLaw()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            DataTable dtResult = lawDocService.GetListLawDoc();
            APIResult rs = new();
            return rs.Success(dtResult);
        }

        [HttpGet("[action]")]
        public APIResult GetLawHTML(int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            DataTable dtResult = lawDocService.GetLawHTML(id);
            APIResult rs = new();
            return rs.Success(dtResult);
        }

        [HttpPost]
        public APIResult PostLaw([FromForm] LawImport lawImport)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            lawDocService.ImportLaw(lawImport.Name, lawImport.Content);
            APIResult rs = new();

            return rs.MessageSuccess("Import văn bản luật thành công");
        }
    }
}
