using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IChapterService chapterService, ILogger<ChapterController> logger)
        {
            _chapterService = chapterService;
            _logger = logger;
        }

        [HttpGet("[action]")]
        public APIResult GetByLawID([BindRequired] int id)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var data = _chapterService.GetListChapterByLawID(id);
            APIResult rs = new APIResult();
            return rs.Success(data);
        }
    }
}
