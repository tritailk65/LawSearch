using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public APIResult GetListConcept()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<Concept> dtResult = _conceptService.GetListConcept();
            APIResult rs = new();
            return rs.Success(dtResult);
        }

        [HttpPost]
        public APIResult AddConcept([FromBody] Concept concept)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            Concept dtConcept = _conceptService.AddConcept(concept);
            APIResult rs = new();
            return rs.Success(dtConcept);
        }

        [HttpPut]
        public APIResult UpdateConcept([FromBody] Concept concept)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            Concept dtConcept = _conceptService.UpdateConcept(concept);
            APIResult rs = new();
            return rs.Success(dtConcept);
        }

        [HttpDelete]
        public APIResult DeleteConcept([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.DeleteConcept(id);
            APIResult rs = new();
            return rs.MessageSuccess("Xóa concept thành công !");
        }

        [HttpGet("[action]")]
        public APIResult GetListKeyPhrases([BindRequired]int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhrase> lst = _conceptService.GetKeyPhrasesFormConceptID(id);
            APIResult rs = new();
            return rs.Success(lst);
        }

        [HttpGet("[action]")]
        public APIResult GenerateKeyPhrase()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.GenerateKeyPhrase();
            APIResult rs = new();
            return rs.MessageSuccess("Generate success!");
        }

        public class dataConceptKeyphrase
        {
            public int conceptid;
            public string keyphrase = "";
        }

        [HttpPost("[action]")]
        public APIResult AddConceptKeyphrase([FromBody] dataConceptKeyphrase d)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<ConceptKeyphrase> lst = _conceptService.AddConceptKeyphrase(d.conceptid, d.keyphrase);
            APIResult rs = new();
            return rs.Success(lst);
        }
    }
}
