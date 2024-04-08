using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
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

        [HttpGet]
        public APIResult GetAllKeyPhrase()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhrase> dtResult = keyPhraseService.GetListKeyPhrase();
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpGet("[action]")]
        public APIResult GetKeyPhraseRelate([BindRequired] int ID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<KeyPhraseRelate> dtResult = keyPhraseService.GetKeyPhraseRelateDetailsByID(ID);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpPost]
        public APIResult AddKeyPhrase([FromBody] KeyPhrase keyPhrase)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            KeyPhrase dtResult = keyPhraseService.AddKeyPhrase(keyPhrase);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpDelete]
        public APIResult DeleteKeyPhrase([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DeleteKeyPhrase(id);
            APIResult result = new APIResult();
            return result.MessageSuccess("Xóa keyphrase thành công!");
        }

        [HttpPost("[action]")]
        public APIResult GenerateKeyphraseMapping([BindRequired] int LawID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.GenerateKeyPhraseMapping(LawID);
            APIResult result = new APIResult();
            return result.MessageSuccess("Generate keyphrase success!");
        }

        [HttpDelete("[action]")]
        public APIResult DeleteAllMapping()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DeleteAllKeyphraseMapping();
            APIResult result = new APIResult();
            return result.MessageSuccess("Delete all KeyphraseMapping success!");
        }

        [HttpDelete("[action]")]
        public APIResult DeleteMapping([BindRequired] int KeyphraseID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            keyPhraseService.DelettKeyphraseMapping(KeyphraseID);
            APIResult result = new APIResult();
            return result.MessageSuccess($"Delete mapping KeyphraseID: {KeyphraseID} success!");
        }
    }
}
