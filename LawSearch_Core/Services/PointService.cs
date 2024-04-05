using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = LawSearch_Core.Models.Point;

namespace LawSearch_Core.Services
{
    public class PointService : IPointService
    {
        private readonly IDbService _dbService;

        public PointService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public List<Point> GetListPointByLawID(int lawID)
        {
            try
            {
                List<Point> lst = new List<Point>();
                _dbService.OpenConnection();

                var sql = @"select * from [Point] where LawID = " + lawID + @" order by id";
                DataTable rs = _dbService.ExecuteReaderCommand(sql, "");
                if (rs.Rows.Count != 0)
                {
                    for (var i = 0; i < rs.Rows.Count; i++)
                    {
                        lst.Add(new Point
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            ClauseID = Globals.GetIDinDT(rs, i, "ClauseID"),
                            Name = Globals.GetinDT_String(rs, i, "Name"),
                            Content = Globals.GetinDT_String(rs, i, "Content"),
                            Number = Globals.GetIDinDT(rs, i, "Number")
                        });
                    }
                }
                return lst;
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
    }
}
