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

        public async Task<string> AddKeyphrase(KeyPhrase keyphrase)
        {
            var body = new
            {
                KeyPhrase = keyphrase.Keyphrase
            };


            var rs = await httpClient.PostAsJsonAsync($"api/KeyPhrase", body);

            if (rs.IsSuccessStatusCode)
            {
                return "Add success!";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    return resultPost.Message.ToString();
                }
            }
            return "An unknown error"; //Lỗi do logic bị sai
        }

        public async Task<string> DeleteKeyphrase(int id)
        {
            var rs = await httpClient.DeleteAsync($"api/KeyPhrase?id="+ id);

            if (rs.IsSuccessStatusCode)
            {
                return "Delete success!";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    return resultPost.Message.ToString();
                }
            }
            return "An unknown error"; //Lỗi do logic bị sai
        }

    }
}
