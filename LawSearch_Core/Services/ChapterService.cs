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
    public class ChapterService: IChapterService
    {
        private readonly IDbService dbService;

        public ChapterService(IDbService dbService)
        {
            this.dbService = dbService;
        }

        public List<Chapter> GetListChapterByLawID(int id)
        {
            try
            {
                List<Chapter> lst = new List<Chapter>();
                dbService.OpenConnection();
                string checkIDLaw = "Select * from law where id = " + id;
                var rsCheckID = dbService.ExecuteReaderCommand(checkIDLaw, "");
                if (rsCheckID.Rows.Count == 0)
                {
                    throw new BadRequestException("Khong tim thay ID law !", 400, 400);
                }

                var sql = @"select * from [Chapter] where lawID = "+id+@" order by id";
                DataTable rs = dbService.ExecuteReaderCommand(sql, "");
                if(rs.Rows.Count != 0)
                {
                    for(var i = 0; i< rs.Rows.Count; i++)
                    {
                        lst.Add(new Chapter
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            Name = Globals.GetinDT_String(rs, i, "Name"),
                            Title = Globals.GetinDT_String(rs, i, "Title"),
                            LawID = Globals.GetIDinDT(rs, i, "LawID"),
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
                dbService.CloseConnection();
            }
        }
    }
}
