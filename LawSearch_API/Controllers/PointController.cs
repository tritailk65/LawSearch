using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly IPointService _pointService;

        public PointController(IPointService pointService)
        {
            _pointService = pointService;
        }

        [HttpGet("[action]"), Authorize]
        public APIResult GetByLawID([BindRequired] int id)
        {
            var dt = _pointService.GetListPointByLawID(id);
            APIResult rs = new APIResult();
            return rs.Success(dt);
        }
    }
}
