using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface ISearchAdminService
    {
        Task<SearchArticalResult> GetResultSearchLaw(string input);
    }
}