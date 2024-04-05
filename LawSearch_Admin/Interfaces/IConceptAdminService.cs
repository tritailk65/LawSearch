using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IConceptAdminService
    {
        Task<List<Concept>> GetListConcept();

        Task<List<KeyPhrase>> GetListKeyphraseByConceptID(int concept_id);

        Task<string> AddConcept(string name, string content);

    }
}
