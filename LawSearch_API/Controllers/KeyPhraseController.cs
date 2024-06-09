using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyPhraseController : Controller
    {
        private readonly IKeyPhraseService keyPhraseService;
        private readonly ILogger<KeyPhraseController> logger;

        public KeyPhraseController(IKeyPhraseService keyPhraseService, ILogger<KeyPhraseController> logger)
        {
            this.keyPhraseService = keyPhraseService;
            this.logger = logger;
        }

        [HttpGet, Authorize]
        public APIResult GetAllKeyPhrase()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhrase> dtResult = keyPhraseService.GetListKeyPhrase();
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetKeyPhraseRelate([BindRequired] int ID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhraseRelate> dtResult = keyPhraseService.GetKeyPhraseRelateDetailsByID(ID);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public APIResult AddKeyPhrase([FromBody] KeyPhrase keyPhrase)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            KeyPhrase dtResult = keyPhraseService.AddKeyPhrase(keyPhrase);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public APIResult DeleteKeyPhrase([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DeleteKeyPhrase(id);
            APIResult result = new APIResult();
            return result.MessageSuccess("Xóa keyphrase thành công!");
        }

        [HttpPost("[action]"), Authorize(Roles = "Admin")]
        public  async Task<APIResult> GenerateKeyphraseVNCoreNLP([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            await keyPhraseService.GenerateKeyphraseVNCoreNLP(LawID);
            APIResult rs = new APIResult();
            return rs.MessageSuccess("Generate success!");
        }

        [HttpPost("[action]"), Authorize(Roles = "Admin")]
        public APIResult GenerateKeyphraseMapping([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.GenerateKeyphraseMapping(LawID);
            APIResult result = new APIResult();
            return result.MessageSuccess("Generate keyphrase mapping success!");
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("DeleteMapping")]
        public APIResult DeleteKeyphraseMapping([BindRequired] int KeyphraseID, [BindRequired] int ArticalID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DeleteKeyPhraseMapping(KeyphraseID, ArticalID);
            APIResult result = new APIResult();
            return result.MessageSuccess("Delete keyphrase mapping success!");
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        [Route("DeleteAllMapping")]
        public APIResult DeleteKeyphraseMapping([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DeleteAllKeyPhraseMapping(LawID);
            APIResult result = new APIResult();
            return result.MessageSuccess("Delete all keyphrase mapping success!");
        }
    }
}
