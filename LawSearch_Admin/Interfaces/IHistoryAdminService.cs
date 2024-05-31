using LawSearch_Core.Models;

namespace LawSearch_Admin.Interfaces
{
    public interface IHistoryAdminService
    {
        Task<List<HistorySearch>> GetHistorySearch(int UserID, DateTime fromDate, DateTime toDate);

        Task DeleteHistorySearch(int UserID, DateTime fromDate, DateTime toDate);

        Task AddHistorySearch(int UserID, string searchString);
    }
}
