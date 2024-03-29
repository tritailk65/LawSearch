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
/*            APIResultVM rs = new APIResultVM();
            rs = await _httpClient.GetFromJsonAsync<APIResultVM>($"/api/Artical");*/
            List<Artical> lst = new List<Artical>();


            return lst;
        }
    }
}
