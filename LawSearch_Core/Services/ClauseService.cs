using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class ClauseService: IClauseService
    {
        private readonly IDbService db;

        public ClauseService(IDbService db)
        {
            this.db = db;
        }

        public List<Clause> GetListClauseByLawID(int id)
        {
            try
            {
                List<Clause> lst = new List<Clause>();
                db.OpenConnection();

                var sql = @"select * from [Clause] where LawID = " + id + @" order by id";
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                if (rs.Rows.Count != 0)
                {
                    for (var i = 0; i < rs.Rows.Count; i++)
                    {
                        lst.Add(new Clause
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            ArticalID = Globals.GetIDinDT(rs, i, "ArticalID"),
                            Title = Globals.GetinDT_String(rs, i, "Title"),
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
                db.CloseConnection();
            }
        }
    }
}
