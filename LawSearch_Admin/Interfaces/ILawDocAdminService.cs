using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface ILawDocAdminService
    {
        Task<List<LawDoc>> GetListLawDoc();
        Task<List<LawHTML>> GetLawHTML(int id);
    }
}
