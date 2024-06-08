using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;

namespace LawSearch_Admin.Services
{
    public class SearchAdminService : ISearchAdminService
    {
        private readonly HttpClient httpClient;
        private readonly ICookieService _cookie;

        public SearchAdminService(HttpClient httpClient, ICookieService cookie)
        {
            this.httpClient = httpClient;
            _cookie = cookie;
        }

        public async Task<SearchResult> GetResultSearchLaw(string input)
        {

            SearchResult lst = new SearchResult();

            var rs = await httpClient.GetFromJsonAsync<APIResultSingleVM<SearchResult>>($"api/Search?input={input}");

            if (rs != null && rs.Status == 200 && rs.Data != null)
            {
                lst = rs.Data;
            }
            return lst;
        }
    }
}