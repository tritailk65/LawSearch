using LawSearch_Admin.Interfaces;
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
            return await _httpClient.GetFromJsonAsync<List<Artical>>($"/api/Artical");
        }
    }
}
