using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Net.Http.Json;

namespace LawSearch_Admin.Services
{
    public class ArticalAdminService : IArticalAdminService
    {
        private readonly HttpClient _httpClient;

        public ArticalAdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Artical>> GetListArtical()
        {
            List<Artical> lst = new List<Artical>();
            var rs = await _httpClient.GetFromJsonAsync<APIResultVM<Artical>>($"/api/Artical");
            if (rs.Status == 200 && rs.Data.Count > 0)
            {
                lst = rs.Data.ToList();
            }

            return lst;
        }
    }
}
