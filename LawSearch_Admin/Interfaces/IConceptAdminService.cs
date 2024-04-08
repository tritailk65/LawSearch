using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IConceptAdminService
    {
        Task<List<Concept>> GetListConcept();

        Task<List<KeyPhrase>> GetListKeyphraseByConceptID(int concept_id);

        Task<ResponceMessage> AddConcept(string name, string content);

        Task<ResponceMessage> UpdateConcept(Concept newConcept);

        Task<ResponceMessage> DeleteConcept(int id);

        Task<ResponceMessage> AddConceptKeyphrase(int conceptid, string keyphrase);
    }
}
