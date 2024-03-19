using LawSearch_Core.Interfaces;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetListLaw()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            DataTable dtResult = lawDocService.GetListLawDoc();
            return Ok(dtResult);
        }

        [HttpGet("[action]")]
        public IActionResult GetLawHTML(int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            DataTable dtResult = lawDocService.GetLawHTML(id);
            return Ok(dtResult);
        }
    }
}
