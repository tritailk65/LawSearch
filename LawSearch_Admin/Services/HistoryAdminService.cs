using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace LawSearch_Admin.Services
{
    public class HistoryAdminService : IHistoryAdminService
    {
        private readonly HttpClient _httpClient;

        public HistoryAdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddHistorySearch(int UserID, string searchString)
        {
            var rs = await _httpClient.PostAsJsonAsync($"api/HistorySearch?UserID={UserID}&SearchString={searchString}", new { });
        }

        public async Task DeleteHistorySearch(int UserID, DateTime fromDate, DateTime toDate)
        {
            string formDateFormat = fromDate.ToString("yyyy-MM-dd");
            string toDateFormat = toDate.AddDays(1).ToString("yyyy-MM-dd");
            var rs = await _httpClient.DeleteAsync($"api/HistorySearch?UserID={UserID}&FromDate={formDateFormat}&ToDate={toDateFormat}");
        }

        public async Task<List<HistorySearch>> GetHistorySearch(int UserID, DateTime fromDate, DateTime toDate)
        {
            List<HistorySearch> historySearches = new List<HistorySearch>();
            string formDateFormat = fromDate.ToString("yyyy-MM-dd");
            string toDateFormat = toDate.AddDays(1).ToString("yyyy-MM-dd");
            var rs = await _httpClient.GetFromJsonAsync<APIResultVM<HistorySearch>>($"api/HistorySearch?UserID={UserID}&FromDate={formDateFormat}&ToDate={toDateFormat}");
            if (rs != null && rs.Status == 200)
            {
                if (rs.Data.Count > 0)
                {
                    historySearches = rs.Data.ToList();
                }
                return historySearches;
            }
            else return null;
        }
    }
}
