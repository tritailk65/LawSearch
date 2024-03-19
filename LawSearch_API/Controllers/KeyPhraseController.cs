using LawSearch_Core.Interfaces;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult GetAllKeyPhrase()
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);

            DataTable dtResult = keyPhraseService.GetListKeyPhrase();
            return Ok(dtResult);

        }

        [HttpGet("[action]")]
        public ActionResult GetKeyPhraseRelate(int ID)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);

            DataTable dtResult = keyPhraseService.GetKeyPhraseRelateDetailsByID(ID);
            return Ok(dtResult);

        }
    }
}
