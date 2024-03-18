using LawSearch_Admin.Interfaces;
using LawSearch_Core.Models;
using System.Net.Http.Json;

namespace LawSearch_Admin.Services
{
    public class LawDocAdminService : ILawDocAdminService
    {
        private readonly HttpClient _httpClient;

        public LawDocAdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LawDoc>> GetListLawDoc()
        {
            return await _httpClient.GetFromJsonAsync<List<LawDoc>>($"api/lawdoc");
        }

        public async Task<List<LawHTML>> GetLawHTML(int id)
        {
            return await _httpClient.GetFromJsonAsync<List<LawHTML>>($"api/lawdoc/GetLawHTML?id={id}");
        }
    }
}
