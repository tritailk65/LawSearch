using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IConceptAdminService
    {
        Task<List<Concept>> GetListConcept();

        Task<List<KeyPhrase>> GetListKeyphraseByConceptID(int concept_id);

        //Code tu interface ra serrvice
        // T code service ok roi moi them vao interface

        // ?? logic DI khong phai v
        Task<Concept> AddConcept(string name, string content);
    }
}
