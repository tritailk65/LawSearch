using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Net.Http.Json;

namespace LawSearch_Admin.Services
{
    public class KeyPhraseAdminService : IKeyPhraseAdminService
    {
        private readonly HttpClient httpClient;

        public KeyPhraseAdminService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<KeyPhraseRelate>> GetKeyPhraseRelates(int id)
        {
            List<KeyPhraseRelate> lst = new List<KeyPhraseRelate>();
            var rs = await httpClient.GetFromJsonAsync<APIResultVM<KeyPhraseRelate>>($"api/keyphrase/GetKeyPhraseRelate?ID={id}");
            if (rs != null && rs.Status == 200 && rs.Data.Count != 0)
            {
                lst = rs.Data.ToList();
            }

            return lst;
        }

        public async Task<List<KeyPhrase>> GetListKeyPhrase()
        {
            List<KeyPhrase> lst = new List<KeyPhrase>();
            var rs = await httpClient.GetFromJsonAsync<APIResultVM<KeyPhrase>>($"api/keyphrase");
            if(rs != null && rs.Status == 200 && rs.Data.Count != 0)
            {
                lst = rs.Data.ToList();
            }
            return lst;
        }
    }
}
