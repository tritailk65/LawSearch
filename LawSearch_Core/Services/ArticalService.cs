using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Services
{
    public class ArticalService : IArticalService
    {
        private readonly IDbService db;

        public ArticalService(IDbService db)
        {
            this.db = db;
        }

        public List<Artical> GetAllArtical()
        {
            try
            {
                List<Artical> lst = new List<Artical>();
                db.OpenConnection();

                string sql = "SELECT * FROM ARTICAL";
                DataTable rs = db.ExecuteReaderCommand(sql, "");        
                return lst;

            }catch 
            {
                throw;
            } finally
            {
                db.CloseConnection();
            }
        }

        public List<Artical> GetListArticalByLawID(int lawID)
        {
            try
            {
                List<Artical> lst = new List<Artical>();
                db.OpenConnection();
                string checkIDLaw = "Select * from Law where id = " + lawID;
                var rsCheckID = db.ExecuteReaderCommand(checkIDLaw, "");
                if (rsCheckID.Rows.Count == 0)
                {
                    throw new BadRequestException("Khong tim thay ID law !", 400, 400);
                }

                var sql = @"select * from [Artical] where LawID = " + lawID + @" order by id";
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                if (rs.Rows.Count != 0)
                {
                    for (var i = 0; i < rs.Rows.Count; i++)
                    {
                        lst.Add(new Artical
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            Name = Globals.GetinDT_String(rs, i, "Name"),
                            Title = Globals.GetinDT_String(rs, i, "Title"),
                            LawID = Globals.GetIDinDT(rs, i, "LawID"),
                            Number = Globals.GetIDinDT(rs, i, "Number"),
                            Content = Globals.GetinDT_String(rs, i, "Content"),
                            ChapterID = Globals.GetIDinDT(rs, i, "ChapterID"),
                            ChapterItemID = Globals.GetIDinDT(rs, i, "ChapterItemID")
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
