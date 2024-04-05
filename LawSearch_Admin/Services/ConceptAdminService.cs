using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace LawSearch_Admin.Services
{
    public class ConceptAdminService : IConceptAdminService
    {
        private readonly HttpClient httpClient;

        public ConceptAdminService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Concept>> GetListConcept()
        {
            List<Concept> lst = new List<Concept>();

            var rs = await httpClient.GetFromJsonAsync<APIResultVM<Concept>>($"api/concept");

            //Handle if Status != 200 here

            if (rs != null && rs.Status == 200)
            {
                if (rs.Data.Count > 0)
                {
                    lst = rs.Data.ToList();
                }
            }

            return lst;
        }

        public async Task<List<KeyPhrase>> GetListKeyphraseByConceptID(int concept_id)
        {
            List<KeyPhrase> ls = new();
            var rs = await httpClient.GetFromJsonAsync<APIResultVM<KeyPhrase>>($"api/Concept/GetListKeyPhrases?id=" + concept_id);

            if (rs != null && rs.Status == 200)
            {
                if (rs.Data.Count > 0)
                {
                    ls = rs.Data.ToList();
                }
                else
                {
                    // Handle error here
                }
            }
            else
            {
                // Handle error here
            }

            return ls;
        }

        public async Task<string> AddConcept(string name, string content)
        {
            Concept newConcept = new Concept()
            {
                Name = name,
                Content = content
            };

            var rs = await httpClient.PostAsJsonAsync($"api/concept", newConcept);

            if (rs.IsSuccessStatusCode)
            {
                return "Thêm concept thành công !";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    return resultPost.Message.ToString();
                }
            }
            return "Lỗi không xác định"; //Lỗi do logic bị sai
        }
    }
}
