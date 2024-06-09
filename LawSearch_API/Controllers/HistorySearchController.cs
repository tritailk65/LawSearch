using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public APIResult GetHistorySearch([BindRequired] int UserID, [BindRequired] string FromDate, [BindRequired] string ToDate)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            var dtResult = _historySearchService.GetHistorySearchByDate(UserID, FromDate, ToDate);
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpGet, Authorize]
        [Route("GetAll")]
        public APIResult GetAllHistorySearch ()
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            var dtResult = _historySearchService.GetAllHistorySearch();
            APIResult result = new APIResult();
            return result.Success(dtResult);
        }

        [HttpPost, Authorize]
        public APIResult AddHistorySearch([BindRequired] int UserID, [BindRequired] string SearchString, [BindRequired] string SearchResult)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            _historySearchService.AddHistorySearch(UserID, SearchString, SearchResult);
            APIResult result = new APIResult();
            return result.MessageSuccess("Add data success!");
        }

        [HttpDelete, Authorize]
        public APIResult DeleteHistorySearch([BindRequired] int UserID, [BindRequired] string FromDate, [BindRequired] string ToDate)
        {
            _logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path + Request.QueryString);
            _historySearchService.DeleteHistorySearch(UserID, FromDate, ToDate);
            APIResult result = new APIResult();
            return result.MessageSuccess("Delete data success!");
        }
    }
}
