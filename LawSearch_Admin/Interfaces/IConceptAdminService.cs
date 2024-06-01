using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IConceptAdminService
    {
        Task<List<Concept>> GetListConcept();

        Task<List<ConceptKeyphraseShow>> GetListKeyphraseByConceptID(int concept_id);

        Task<ResponseMessage> AddConcept(string name, string content);

        Task<ResponseMessage> UpdateConcept(Concept newConcept);

        Task<ResponseMessage> DeleteConcept(int id);

        Task<ResponseMessage> AddConceptKeyphrase(int conceptid, string keyphrase);

        Task<ResponseMessage> DeleteConceptKeyphrase(ConceptKeyphraseShow k);

        Task<bool> DeleteConceptMapping(int LawID);

        Task<bool> GenerateKeyphraseDescript(int conceptID);
    }
}
