using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
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

        public async Task<List<HistorySearch>> GetHistorySearch(int UserID, DateTime fromDate, DateTime toDate)
        {
            List<HistorySearch> historySearches = new List<HistorySearch>();
            string formDateFormat = fromDate.ToString("yyyy-MM-dd");
            string toDateFormat = toDate.ToString("yyyy-MM-dd");
            var rs = await _httpClient.GetFromJsonAsync<APIResultVM<HistorySearch>>($"api/HistorySearch?UserID={1}&FromDate={formDateFormat}&ToDate={toDateFormat}");
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
