using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class HistorySearchService : IHistorySearchService
    {
        private readonly IDbService _dbService;

        public HistorySearchService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public List<HistorySearch> GetHistorySearchByDate(int UserID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _dbService.OpenConnection();

                var checkUserID = _dbService.ExecuteReaderCommand($"select * from [User] where ID = {UserID}","") ;
                if(checkUserID.Rows.Count == 0)
                {
                    throw new BadRequestException("UserID not found!", 404, 400);
                }    

                string fromDateFormat = fromDate.ToString("yyyy-MM-dd");
                string toDateFormat = toDate.ToString("yyyy-MM-dd");

                List<HistorySearch> rs = new List<HistorySearch>();
                string sql = $"select q.ID, q.Value, q.Result, q.Datetime, u.Username from Query q " +
                                $"inner join [User] u on u.ID = q.UserID  " +
                                $"where UserID = {UserID} and (DateTime >= '{fromDateFormat}' and DateTime <= '{toDateFormat}') " +
                                $"order by Datetime DESC";
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                if(dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rs.Add(new HistorySearch
                        {
                            ID = Globals.GetIDinDT(dt, i, "ID"),
                            Value = Globals.GetinDT_String(dt, i, "Value"),
                            DateTime = DateTime.ParseExact(Globals.GetinDT_String(dt, i, "DateTime"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            UserName = Globals.GetinDT_String(dt, i, "UserName")
                        });
                    }
                }
                return rs;
            }catch
            {
                throw;
            }
            finally
            {
                _dbService.CloseConnection();
            }
        }

    }
}
