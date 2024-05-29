using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface IHistorySearchService
    {
        List<HistorySearch> GetHistorySearchByDate(int UserID, DateTime fromDate, DateTime toDate);

        void AddHistorySearch(int UserID, string searchString);
    }
}
