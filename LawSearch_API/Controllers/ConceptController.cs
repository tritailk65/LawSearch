using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet, Authorize]
        public APIResult GetListConcept()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<Concept> dtResult = _conceptService.GetListConcept();
            APIResult rs = new();
            return rs.Success(dtResult);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public APIResult AddConcept([FromBody] Concept concept)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            Concept dtConcept = _conceptService.AddConcept(concept);
            APIResult rs = new();
            return rs.Success(dtConcept);
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public APIResult UpdateConcept([FromBody] Concept concept)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            Concept dtConcept = _conceptService.UpdateConcept(concept);
            APIResult rs = new();
            return rs.Success(dtConcept);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public APIResult DeleteConcept([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.DeleteConcept(id);
            APIResult rs = new();
            return rs.MessageSuccess("Delete Concept success !");
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetListKeyPhrases([BindRequired]int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhrase> lst = _conceptService.GetKeyPhrasesFormConceptID(id);
            APIResult rs = new();
            return rs.Success(lst);
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetConceptKeyphrase([BindRequired] int ConceptID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<ConceptKeyphraseShow> lst = _conceptService.GetConceptKeyphraseByConceptID(ConceptID);
            APIResult rs = new();
            return rs.Success(lst);
        }

        [HttpPost("[action]"), Authorize(Roles = "Admin")]
        public async Task<APIResult> GenerateKeyphraseDescript([BindRequired] int ConceptID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            await _conceptService.GenerateKeyphraseDescript(ConceptID);
            APIResult rs = new();
            return  rs.MessageSuccess("Generate keyphrase descript success!");
        }

        [HttpPost("[action]"), Authorize(Roles = "Admin")]
        public APIResult GenerateConceptMapping([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.GenerateConceptMapping(LawID);
            APIResult rs = new();
            return rs.MessageSuccess("Generate ConceptMapping success!");
        }

        [HttpDelete("[action]"), Authorize(Roles = "Admin")]
        public APIResult DeleteConceptKeyphrase([BindRequired] int ConceptKeyphraseID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.DeleteConceptKeyphrase(ConceptKeyphraseID);
            APIResult rs = new();
            return rs.MessageSuccess("Delete ConceptKeyphrase success!");
        }

        [HttpPost("[action]"), Authorize(Roles = "Admin")]
        public APIResult AddConceptKeyphrase([FromForm] AddConceptKeyphrase addConceptKeyphrase)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
             _conceptService.AddConceptKeyphrase(addConceptKeyphrase.ConceptID, addConceptKeyphrase.Keyphrase);
            APIResult rs = new();
            return rs.MessageSuccess("Add success !");
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("DeleteMapping")]
        public APIResult DeleteConceptMapping([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            _conceptService.DeleteConceptMapping(LawID);
            APIResult rs = new();
            return rs.MessageSuccess("Delete Concept mapping success !");
        }
    }
}
