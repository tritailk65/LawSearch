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
    public class HistorySearchController : ControllerBase
    {
        private readonly IHistorySearchService _historySearchService;
        private readonly ILogger<HistorySearchController> _logger;

        public HistorySearchController(IHistorySearchService historySearchService, ILogger<HistorySearchController> logger)
        {
            _historySearchService = historySearchService;
            _logger = logger;
        }

        [HttpGet, Authorize]
        public APIResult GetAllHistorySearchRecently([BindRequired] int UserID, [BindRequired] DateTime FromDate, [BindRequired] DateTime ToDate)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            var dtResult = _historySearchService.GetHistorySearchByDate(UserID, FromDate, ToDate);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

    }
}
