using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace LawSearch_Admin.Services
{
    public class KeyPhraseAdminService : IKeyPhraseAdminService
    {
        private readonly HttpClient httpClient;
        private readonly ICookieService _cookie;

        public KeyPhraseAdminService(HttpClient httpClient, ICookieService cookie)
        {
            this.httpClient = httpClient;
            this._cookie = cookie;
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

        public async Task<ResponseMessage> AddKeyphrase(String keyphraseText)
        {
            KeyPhrase k = new()
            {
                Keyphrase = keyphraseText
            };

            HttpResponseMessage rs = await httpClient.PostAsJsonAsync($"api/KeyPhrase", k);
            ResponseMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                rm.StatusAPI = true;
                rm.Message = "Add Keyphrase Success";
                try
                {
                    APIResultSingleVM apiResponse = await rs.Content.ReadAsAsync<APIResultSingleVM>();
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
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultSingleVM>().Result;
                
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Add Keyphrase Failed";
                    rm.Error = resultPost.Message.ToString();
                }

            }
            return rm;
        }

        public async Task<ResponseMessage> DeleteKeyphrase(int id)
        {

            var rs = await httpClient.DeleteAsync($"api/KeyPhrase?id=" + id);
            ResponseMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                rm.StatusAPI = true;
                rm.Message = "Delete KeyPhrase Success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultSingleVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Delete Keyphrase Failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }
            return rm;
        }

        public async Task<ResponseMessage> DeleteKeyphraseMapping(int KeyphraseID, int ArticalID)
        {
            var rs = await httpClient.DeleteAsync($"api/KeyPhrase/DeleteMapping?KeyphraseID={KeyphraseID}&ArticalID={ArticalID}");
            ResponseMessage rm = new();
            if (rs.IsSuccessStatusCode)
            {
                rm.StatusAPI = true;
                rm.Message = "Delete KeyPhrase Mapping Success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultSingleVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Delete Keyphrase Mapping Failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }
            return rm;
        }

        public async Task<bool> DeleteAllKeyphraseMapping(int LawID)
        {
            try
            {
                var rs = await httpClient.DeleteAsync($"api/KeyPhrase/DeleteMapping?LawID={LawID}");
                if (rs.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
