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
    public class SectionService: ISectionService
    {
        private readonly IDbService _db;

        public SectionService(IDbService db)
        {
            _db = db;
        }

        public void EditContentSection(Section section)
        {
            #region Transaction init
            IDbConnection connection = _db.GetDbConnection();
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message.ToString(), 500);
            }
            IDbCommand command = _db.CreateCommand();
            IDbTransaction transaction = _db.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            #endregion

            try
            {
                //Check data
                command.CommandText = $"Select * From ChapterItem Where ID = {section.ID}";
                var checkId = _db.ExecuteReaderCommand(command,"");
                if (checkId.Rows.Count == 0)
                {
                    throw new BadRequestException("Không tìm thấy ID Section", 400, 400);
                }

                if (section.Name == null)
                {
                    throw new BadRequestException("Nội dung chỉnh sửa không được bỏ trống", 400, 400);
                }
                

                command.CommandText = $"Update ChapterItem " +
                                        $"Set Name = N'{section.Name}' " +
                                        $"Where ID = {section.ID}";
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        public List<Section> GetByLawID(int id)
        {
            try
            {
                List<Section> lst = new List<Section>();
                _db.OpenConnection();
                string checkIDLaw = "Select * from Law where id = " + id;
                var rsCheckID = _db.ExecuteReaderCommand(checkIDLaw, "");
                if (rsCheckID.Rows.Count == 0)
                {
                    throw new BadRequestException("Khong tim thay ID Law !", 400, 400);
                }

                var sql = @"select * from [ChapterItem] where LawID = " + id + @" order by id";
                DataTable rs = _db.ExecuteReaderCommand(sql, "");
                if (rs.Rows.Count != 0)
                {
                    for (var i = 0; i < rs.Rows.Count; i++)
                    {
                        lst.Add(new Section
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            Name = Globals.GetinDT_String(rs, i, "Name"),
                            Number = Globals.GetIDinDT(rs, i, "Number"),
                            ChapterID = Globals.GetIDinDT(rs, i, "ChapterID"),
                            LawID = Globals.GetIDinDT(rs, i, "LawID"),
                            Title = Globals.GetinDT_String(rs, i, "Name"),
                        });
                    }
                }
                return lst;
            } catch
            {
                throw;
            } finally
            {
                _db.CloseConnection();
            }
        }
    }
}
