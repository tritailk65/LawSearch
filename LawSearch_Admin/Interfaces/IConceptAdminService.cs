using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IConceptAdminService
    {
        Task<List<Concept>> GetListConcept();
    }
}
