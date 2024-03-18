using LawSearch_Admin.Interfaces;
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
            return await httpClient.GetFromJsonAsync<List<KeyPhraseRelate>>($"api/keyphrase/GetKeyPhraseRelate?ID={id}");
        }

        public async Task<List<KeyPhrase>> GetListKeyPhrase()
        {
            return await httpClient.GetFromJsonAsync<List<KeyPhrase>>($"api/keyphrase");
        }
    }
}
