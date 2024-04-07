using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

        public async Task<ResponceMessage> AddConcept(string name, string content)
        {
            Concept newConcept = new()
            {
                Name = name,
                Content = content
            };

            ResponceMessage rm = new();

            var rs = await httpClient.PostAsJsonAsync($"api/concept", newConcept);

            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Add concept success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Update concept failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }
            return rm;
        }

        public async Task<ResponceMessage> UpdateConcept(Concept newConcept)
        {
            ResponceMessage rm = new();

            var rs = await httpClient.PutAsJsonAsync($"api/Concept", newConcept);

            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Update concept success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Update concept failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }

            return rm;
        }

        public async Task<ResponceMessage> DeleteConcept(int id)
        {
            ResponceMessage rm = new();

            var rs = await httpClient.DeleteAsync($"api/Concept?id=" + id);

            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Delete concept success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Delete concept failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }

            return rm;
        }

        public async Task<ResponceMessage> AddConceptKeyphrase(int conceptid, string keyphrase)
        {
            ResponceMessage rm = new();

            var body = new
            {
                conceptid = conceptid,
                keyphrase = keyphrase
            };
            var rs = await httpClient.PostAsJsonAsync($"/api/Concept/AddConceptKeyphrase", body);

            if (rs.IsSuccessStatusCode)
            {
                rm.Status = true;
                rm.Message = "Add concept keyphrase success";
            }
            else
            {
                var resultPost = rs.Content.ReadFromJsonAsync<APIResultVM>().Result;
                if (resultPost != null && resultPost.Message != null)
                {
                    rm.Message = "Add concept keyphrase failed";
                    rm.Error = resultPost.Message.ToString();
                }
            }

            return rm;
        }
    }
}
