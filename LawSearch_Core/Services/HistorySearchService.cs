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

        public void AddHistorySearch(int UserID, string searchString, string searchResult)
        {
            try
            {
                _dbService.OpenConnection();

/*                var stringResult = $"TOP 1 - Điều 1: Phạm vi điều chỉnh\n" +
                    $"TOP 2 - Điều 2: Đối tượng áp dụng\n" +
                    $"TOP 3 - Điều 3: Giải thích từ ngữ";*/

                var checkUserID = _dbService.ExecuteReaderCommand($"select * from [User] where ID = {UserID}", "");
                if (checkUserID.Rows.Count == 0)
                {
                    throw new BadRequestException("UserID not found!", 404, 400);
                }

                string sql = $"exec QueryLog N'{searchString}', N'{searchResult}' , {UserID}";
                _dbService.ExecuteNonQueryCommand(sql);

            } catch
            {
                throw;
            }
            finally
            {
                _dbService.CloseConnection();
            }
        }

        public void DeleteHistorySearch(int UserID, string fromDate, string toDate)
        {
            try
            {
                _dbService.OpenConnection();

                var checkUserID = _dbService.ExecuteReaderCommand($"select * from [User] where ID = {UserID}","") ;
                if(checkUserID.Rows.Count == 0)
                {
                    throw new BadRequestException("UserID not found!", 404, 400);
                }    

                string sql = $"delete query where UserID = {UserID} and (DateTime >= '{fromDate}' and DateTime < '{toDate}')";
                _dbService.ExecuteNonQueryCommand(sql);

            }catch
            {
                throw;
            }
            finally
            {
                _dbService.CloseConnection();
            }
        }

        public List<HistorySearch> GetAllHistorySearch()
        {
            try
            {
                _dbService.OpenConnection();
                List<HistorySearch> rs = new List<HistorySearch>();
                string sql = $"select q.ID, q.Value, q.Result, q.Datetime, q.Count, q.Result, u.Username from Query q " +
                                $"inner join [User] u on u.ID = q.UserID  " +                             
                                $"order by Datetime DESC";
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DateTime dateTime;
                        if (!DateTime.TryParse(Globals.GetinDT_String(dt, i, "DateTime"), out dateTime))
                        {
                            dateTime = DateTime.Now;
                        }

                        rs.Add(new HistorySearch
                        {
                            ID = Globals.GetIDinDT(dt, i, "ID"),
                            Value = Globals.GetinDT_String(dt, i, "Value"),
                            DateTime = dateTime,
                            Count = Globals.GetIDinDT(dt, i, "Count"),
                            UserName = Globals.GetinDT_String(dt, i, "UserName"),
                            Result = Globals.GetinDT_String(dt,i,"Result")
                        });
                    }
                }
                return rs;
            }
            catch
            {
                throw;
            }
            finally
            {
                _dbService.CloseConnection();
            }
        }

        public List<HistorySearch> GetHistorySearchByDate(int UserID, string fromDate, string toDate)
        {
            try
            {
                _dbService.OpenConnection();

                var checkUserID = _dbService.ExecuteReaderCommand($"select * from [User] where ID = {UserID}","") ;
                if(checkUserID.Rows.Count == 0)
                {
                    throw new BadRequestException("UserID not found!", 404, 400);
                }    

/*                string fromDateFormat = fromDate.ToString("yyyy-MM-dd");
                string toDateFormat = toDate.AddDays(1).ToString("yyyy-MM-dd");*/

                List<HistorySearch> rs = new List<HistorySearch>();
                string sql = $"select q.ID, q.Value, q.Result, q.Datetime, q.Count, q.Result, u.Username from Query q " +
                                $"inner join [User] u on u.ID = q.UserID  " +
                                $"where UserID = {UserID} and (DateTime >= '{fromDate}' and DateTime < '{toDate}') " +
                                $"order by Datetime DESC";
                var dt = _dbService.ExecuteReaderCommand(sql, "");
                if(dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DateTime dateTime;
                        if (!DateTime.TryParse(Globals.GetinDT_String(dt, i, "DateTime"), out dateTime))
                        {
                            dateTime = DateTime.Now;
                        }

                        rs.Add(new HistorySearch
                        {
                            ID = Globals.GetIDinDT(dt, i, "ID"),
                            Value = Globals.GetinDT_String(dt, i, "Value"),
                            DateTime = dateTime,
                            Count = Globals.GetIDinDT(dt, i, "Count"),
                            UserName = Globals.GetinDT_String(dt, i, "UserName"),
                            Result = Globals.GetinDT_String(dt, i, "Result")
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
