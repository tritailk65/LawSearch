using LawSearch_API.Utils;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using LawSearch_Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LawSearch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            _searchService = searchService;
            this.logger = logger;
        }

        [HttpGet]
        public APIResult SearchArticalByText([BindRequired] string input)
        {
            logger.LogInformation(Request.Method + " " + Request.Scheme + "://" + Request.Host + Request.Path);
            List<ArticalResult> dtResult = _searchService.SearchLawByText(input);
            APIResult rs = new();
            return rs.Success(dtResult);
        }
    }
}
