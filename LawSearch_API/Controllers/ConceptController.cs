using LawSearch_Core.Interfaces;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConceptController : ControllerBase
    {
        private readonly IConceptService _conceptService;
        private readonly ILogger<ConceptController> logger;

        public ConceptController(IConceptService conceptService, ILogger<ConceptController> logger)
        {
            _conceptService = conceptService;
            this.logger = logger;
        }

        // GET: api/<ConceptController>
        [HttpGet]
        public IActionResult Get()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);

            DataTable dtResult = _conceptService.GetListConcept();

            /*            APIResult rs = new APIResult();*/
            return Ok(dtResult);
        }
    }
}
