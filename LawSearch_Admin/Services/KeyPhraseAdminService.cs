using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Newtonsoft.Json;
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
            if (rs != null && rs.Status == 200 && rs.Data.Count != 0)
            {
                lst = rs.Data.ToList();
            }
            return lst;
        }

        public async Task<ResponceMessage> AddKeyphrase(String keyphraseText)
        {
            KeyPhrase k = new()
            {
                Keyphrase = keyphraseText
            };
            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/KeyPhrase", k);
            ResponceMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Add Keyphrase Success";
                try
                {
                    APIResultVM apiResponse = await rs.Content.ReadAsAsync<APIResultVM>();
                    var a = apiResponse;
                    if (apiResponse.Data != null)
                    {
                        KeyPhrase rs_k = JsonConvert.DeserializeObject<KeyPhrase>(apiResponse.Data);
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Add Keyphrase Failed";
                    rm.Error = resultPost.Message.ToString();
                }

            }
            return rm;
        }

        public async Task<ResponceMessage> DeleteKeyphrase(int id)
        {
            var rs = await httpClient.DeleteAsync($"api/KeyPhrase?id=" + id);
            ResponceMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Delete KeyPhrase Success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Delete Keyphrase Failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }
            return rm;
        }

    }
}
