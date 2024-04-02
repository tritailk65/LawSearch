using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface ISearchAdminService
    {
        Task<List<ArticalResult>> GetResultSearchLaw(string input);
    }
}