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

        public void EditContentArtical(Artical artical)
        {
            #region Transaction init
            IDbConnection connection = db.GetDbConnection();
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
            IDbCommand command = db.CreateCommand();
            IDbTransaction transaction = db.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            #endregion

            try
            {
                //Check data
                command.CommandText = $"Select * From Artical Where ID = {artical.ID}";
                var checkId = db.ExecuteReaderCommand(command, "");
                if (checkId.Rows.Count == 0)
                {
                    throw new BadRequestException("Không tìm thấy ID Chapter", 400, 400);
                }

                if (artical.Name == null || artical.Title == "" || artical.Content == "")
                {
                    throw new BadRequestException("Nội dung chỉnh sửa không được bỏ trống", 400, 400);
                }

                command.CommandText = $"Update Artical " +
                                        $"Set Name = N'{artical.Name}', " +
                                        $"Title = N'{artical.Title}', " +
                                        $"Content = N'{artical.Content} '" +
                                        $"Where ID = {artical.ID}";
                db.ExecuteNonQueryCommand(command);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
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

        public ArticalDetail GetArticalDetail(int id)
        {
            try
            {              
                db.OpenConnection();

                string sql = $"Select * From Artical Where ID = {id}";
                var checkId = db.ExecuteReaderCommand(sql, "");
                if (checkId.Rows.Count == 0)
                {
                    throw new BadRequestException("Không tìm thấy ID Artical", 400, 400);
                }

                string sqlDetail = @"Select a.ID, 
		                                    a.Name, 
		                                    a.Title, 
		                                    a.Number, 
		                                    a.Content,
		                                    l.Name as 'LawName',
		                                    c.Name as 'ChapterName', 
		                                    c.Title as 'ChapterTitle', 
		                                    ci.Name as 'SectionName' 
                                    From Artical a
                                    inner join Law l on l.ID = a.LawID
                                    inner join Chapter c on c.ID = a.ChapterID
                                    inner join ChapterItem ci on ci.ID = a.ChapterItemID
                                    Where a.ID = " + id;

                DataTable rs = db.ExecuteReaderCommand(sqlDetail, "");
                ArticalDetail articalDetail = new ArticalDetail
                {
                    ID = Globals.GetIDinDT(rs,0,"ID"),
                    Name = Globals.GetinDT_String(rs, 0, "Name"),
                    Title = Globals.GetinDT_String(rs, 0, "Title"),
                    Number = Globals.GetIDinDT(rs, 0, "Number"),
                    Content = Globals.GetinDT_String(rs, 0, "Content"),
                    LawName = Globals.GetinDT_String(rs, 0, "LawName"),
                    ChapterName = Globals.GetinDT_String(rs,0,"ChapterName"),
                    ChapterTitle = Globals.GetinDT_String(rs, 0, "ChapterTitle"),
                    SectionName = Globals.GetinDT_String(rs, 0, "SectionName")
                };

                return articalDetail;
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
