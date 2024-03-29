using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;

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


    }
}
