using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
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
                if(rs.Data.Count > 0)
                {
                    lst = rs.Data.ToList();
                }
            }

            return lst;
        }

        public async Task<List<KeyPhrase>> GetListKeyphraseByConceptID(int concept_id)
        {
            List<KeyPhrase> ls = new();
            var rs = await httpClient.GetFromJsonAsync<APIResultVM<KeyPhrase>>($"api/Concept/GetListKeyPhrases?id="+concept_id);

            if (rs != null && rs.Status == 200)
            {
                if (rs.Data.Count > 0)
                {
                    ls = rs.Data.ToList();
                } else
                {
                    // Handle error here
                }
            } else
            {
                // Handle error here
            }

            return ls;
        }

        public async Task<Concept> AddConcept(string name, string content)
        {
            Concept c = new Concept();
            var data = new { Name = name, Content = content };
            //Khong can phai khai bao URL o day vi da dinh danh trong appsetting rooi
            // TAI VI CAI BINH THUONG M DAU CHO T EXAMPLE M CHO GET THOI CO CHO POST DAU

            //code di r t chinh lai
            string local_host = "http://localhost:8080";
            var body = JsonSerializer.Serialize(data);


            var rs = await httpClient.PostAsJsonAsync<Concept>($"api/Concept", con);

            if(rs != null)
            {

            }

            var debug = rs;

/*            if(rs != null && rs.Status == 200)
            {

            }*/
            return c;
        }
    }
}
