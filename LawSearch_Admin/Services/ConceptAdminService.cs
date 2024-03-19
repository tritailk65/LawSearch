using LawSearch_Admin.Interfaces;
using LawSearch_Core.Models;
using System.Net.Http.Json;

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
             return await httpClient.GetFromJsonAsync<List<Concept>>($"api/concept");
        }


    }
}
