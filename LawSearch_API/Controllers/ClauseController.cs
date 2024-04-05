using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClauseController : ControllerBase
    {
        private readonly IClauseService _clauseService;
        private readonly ILogger<ClauseController> _logger;

        public ClauseController(IClauseService clauseService, ILogger<ClauseController> logger)
        {
            _clauseService = clauseService;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public APIResult GetByLawID([BindRequired] int id)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var dt = _clauseService.GetListClauseByLawID(id);
            APIResult rs = new APIResult();
            return rs.Success(dt);
        }
    }
}
