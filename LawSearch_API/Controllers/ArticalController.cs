using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/<ArticalController>
        [HttpGet]
        public ActionResult GetAllArtical()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);

            DataTable dtResult = _articalService.GetAllArtical();

/*            APIResult rs = new APIResult();*/
            return Ok(dtResult);

        }

    }
}
