using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace LawSearch_Admin.Services
{
    public class SearchAdminService : ISearchAdminService
    {
        private readonly HttpClient httpClient;
        private readonly ICookie _cookie;

        public SearchAdminService(HttpClient httpClient, ICookie cookie)
        {
            this.httpClient = httpClient;
            _cookie = cookie;
        }

        public async Task<SearchResult> GetResultSearchLaw(string input)
        {
            SearchResult lst = new SearchResult();

            //var authToken = await _cookie.GetValue(CookieKeys.authToken);
            //httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);

            var rs = await httpClient.GetFromJsonAsync<APIResultSingleVM<SearchResult>>($"api/Search?input={input}");

            if (rs != null && rs.Status == 200 && rs.Data != null)
            {
                lst = rs.Data;
            }
            return lst;
        }
    }
}