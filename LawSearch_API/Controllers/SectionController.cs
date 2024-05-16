using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService sectionService;
        private readonly ILogger<SectionController> logger;

        public SectionController(ISectionService sectionService, ILogger<SectionController> logger)
        {
            this.sectionService = sectionService;
            this.logger = logger;
        }

        [HttpGet("[action]")]
        public APIResult GetByLawID([BindRequired] int id)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var data = sectionService.GetByLawID(id);
            APIResult rs = new();
            return rs.Success(data);
        }

        [HttpPut]
        public APIResult EditContentSection([FromBody] Section section)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            sectionService.EditContentSection(section);
            APIResult rs = new();
            return rs.MessageSuccess("Chỉnh sửa nội dung section thành công!");
        }

    }
}
