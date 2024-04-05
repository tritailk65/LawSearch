using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface ILawDocAdminService
    {
        Task<List<LawDoc>> GetListLawDoc();
        Task<List<LawHTML>> GetLawHTML(int id);
        Task<LawVM> GetDataLaw(int id);
        Task<bool> ImportLaw(string name, string content);
        Task<bool> DeleteLaw(int id);
    }
}
