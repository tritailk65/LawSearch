using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using LawSearch_Core.Models;
using System.Data;

namespace LawSearch_Core.Services
{
    public class LawDocService : ILawDocService
    {
        private readonly IDbService db;
        private readonly IKeyPhraseService keyPhraseService;
        private readonly IConceptService conceptService;

        public LawDocService(IDbService db, IKeyPhraseService keyPhraseService, IConceptService conceptService)
        {
            this.db = db;
            this.keyPhraseService = keyPhraseService;
            this.conceptService = conceptService;
        }

        public DataTable GetLawHTML(int ID)
        {
            try
            {
                db.OpenConnection();
                var sql = "select top 1 ContentHTML from [LawHTML] where LawID = " + ID;
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                return rs;
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

        public List<LawDoc> GetListLawDoc()
        {
            try
            {
                List<LawDoc> lawDocs = new List<LawDoc>();
                db.OpenConnection();
                var sql = "select ID, name, status from Law where status = 1 order by ID";
                DataTable rs = db.ExecuteReaderCommand(sql, "");
                if(rs.Rows.Count > 0)
                {
                    for(var i =0;i<rs.Rows.Count; i++)
                    {
                        lawDocs.Add(new LawDoc
                        {
                            ID = Globals.GetIDinDT(rs, i, "ID"),
                            Name = Globals.GetinDT_String(rs, i, "Name"),
                            Status = Globals.GetinDT_String(rs, i, "Status")
                        });
                    }
                }
                return lawDocs;
            }catch
            {
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
        }

        public void ImportLaw(string name, string content)
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
                //Step 1: Parse document
                LawDoc l = new LawDoc();
                ParseContentLaw(command, true, content, name);

                transaction.Commit();
            } catch 
            {              
                transaction.Rollback();
                throw;
            }
            finally
            {
                db.CloseConnection();
            }
        }

        //Parse Law
        private void ParseContentLaw(IDbCommand command, bool isGenerateNewID, string content, string name)
        {
            if (content == "") return;
            LawDoc lawDoc = new LawDoc();
            lawDoc.Name = name;

            command.CommandText = "exec GetLaw N'" + lawDoc.Name + "'";
            lawDoc.ID = Globals.GetIDinDT(db.ExecuteReaderCommand(command, ""), 0, 0);

            //Nếu ID trả về > 0 --> tiến hành import
            if (lawDoc.ID > 0)
            {
                int chapterNumber = 1;
                lawDoc.lstChapters = new List<Chapter>();
                Chapter item = new Chapter();

                while (ParseContentChapter(command, isGenerateNewID, content, chapterNumber++, lawDoc.ID))
                {
                    lawDoc.lstChapters.Add(item);
                    item = new Chapter();
                }
            }
        }

        //Parse Chapter
        private bool ParseContentChapter(IDbCommand command, bool isGenerateNewID, string content, int number, int LawID)
        {
            string nameLama = "CHƯƠNG " + Globals.GetLama(number);
            string nameNormal = "CHƯƠNG " + number + ".";
            Chapter chapter = new Chapter();
            chapter.Name = nameLama;
            chapter.Number = number;
            chapter.LawID = LawID;
            string chapterContent = Globals.NextItem(content, nameLama);
            bool isChuongNormal = false;
            if (chapterContent == "")
            {
                chapterContent = Globals.NextItem(content, nameNormal);
                if (chapterContent != "")
                {
                    isChuongNormal = true;
                    chapter.Name = nameNormal;
                }
            }
            chapterContent = Globals.GetContentBetween(content, chapter.Name, "CHƯƠNG " + (isChuongNormal ? (number + 1).ToString() : Globals.GetLama(number + 1)), true, true);
            if (chapterContent == "" && number == 1)
            {
                if (content.Contains("Điều 1.") || content.Contains("Điều 1:"))
                    chapterContent = content;
                chapter.Number = 0; chapter.Name = ""; number = 0; chapter.Title = "";
            }
            if (chapterContent == "") return false;
            string dieu = chapter.GetTextSignBeginArtical(chapterContent);
            if (string.IsNullOrEmpty(dieu))
            {
                if (chapterContent.Contains("Điều 1."))
                    dieu = "Điều 1.";
                else if (chapterContent.Contains("Điều 1:"))
                    dieu = "Điều 1:";
                else dieu = "\r\n";
            }

            string muc = chapterContent.IndexOf("MỤC 1.", StringComparison.InvariantCultureIgnoreCase) > 0 ? "MỤC 1." : "MỤC 1:";
            if (number > 0)
                chapter.Title = Globals.GetContentBetween(chapterContent, chapter.Name, muc, false, true, dieu, "\r\n");

            command.CommandText = "exec GetChapter N'" + chapter.Name + "', N'" + chapter.Title + "'," + number + "," + LawID;
            chapter.ID = !isGenerateNewID ? -1 : Globals.GetIDinDT(db.ExecuteReaderCommand(command,""), 0, 0);
            if (chapter.ID > 0 || !isGenerateNewID)
            {
                chapter.lstSection = new List<Section>();
                Section item = new Section();
                int ChapterItemNumber = 1;
                while (ParseContentSection(command, isGenerateNewID, chapterContent, ChapterItemNumber, chapter.ID, chapter.Number))
                {
                    chapter.lstSection.Add(item); item = new Section();
                    ChapterItemNumber++;
                }

                return true;
            }

            return false;
        }

        //Parse Section
        private bool ParseContentSection(IDbCommand command, bool isGenerateNewID, string chapterContent, int number, int ChapterID, int ChapterNumber)
        {
            Section section = new Section();
            string tail = ".";
            section.ChapterID = ChapterID; string ChapterItemContent = "";
            section.ChapterNumber = ChapterNumber;

            if (chapterContent.IndexOf("MỤC " + ChapterNumber, StringComparison.InvariantCultureIgnoreCase) < 0
                && chapterContent.IndexOf("MỤC 1\r\n", StringComparison.InvariantCultureIgnoreCase) < 0
                && chapterContent.IndexOf("MỤC 1.", StringComparison.InvariantCultureIgnoreCase) < 0
                && chapterContent.IndexOf("MỤC 1:", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                if (number > 1)
                    return false;
                else
                {
                    section.Number = 0; // muc 0
                    section.Name = "";
                    ChapterItemContent = chapterContent;
                }
            }
            else
            {
                section.Name = "MỤC " + number;

                section.Number = number;
                //if (chapterContent.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && chapterContent.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0)
                tail = Globals.GetTail(chapterContent, section.Name);
                //if (string.IsNullOrEmpty(ChapterItemContent))
                {
                    ChapterItemContent = Globals.NextItem(chapterContent, section.Name);
                    ChapterItemContent = Globals.GetContentBetween(chapterContent, section.Name, "MỤC " + (number + 1), true, true);
                }

            }
            if (ChapterItemContent == "") return false;

            command.CommandText = "exec GetChapterItem N'" + section.Name + "'," + section.Number + ", " + ChapterID;
            section.ID = !isGenerateNewID ? -1 : Globals.GetIDinDT(db.ExecuteReaderCommand(command,""), 0, 0);
            if (section.ID > 0 || !isGenerateNewID)
            {
                section.lstArtical = new List<Artical>();
                Artical item = new Artical();
                int tempArticalnumber = section.GetFirstArticalNumber(ChapterItemContent, section.Number);
                if (tempArticalnumber > 0 && section.Number > 0)
                {
                    section.Title = Globals.GetContentBetween(chapterContent, "\nMỤC " + (section.Number), "\nĐiều " + tempArticalnumber + Globals.GetTail(chapterContent, "\nĐiều " + tempArticalnumber), false, true);
                }
                else section.Title = Globals.GetContentBetween(chapterContent, "\nMỤC " + (section.Number), "\r\n", false, true); section.Title = section.Title.Trim('.').Trim(':').Trim();
                while (tempArticalnumber > 0 && ParseContentArtical(command, isGenerateNewID, ChapterItemContent, tempArticalnumber, section.ID, ChapterID, ChapterNumber))
                {
                    tempArticalnumber++;
                    section.lstArtical.Add(item); item = new Artical();
                }
                if (section.lstArtical.Count == 0)
                {
                    string s = "";
                }
                return true;
            }

            return false;
        }

        //Parse Artical
        private bool ParseContentArtical(IDbCommand command, bool isGenerateNewID, string content, int number, int chapterItemID, int chapterID, int ChapterNumber)
        {
            Artical artical = new Artical();
            artical.Name = "Điều " + number;
            string tail = Globals.GetTail(content, artical.Name);// content.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && content.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0 ? ":" : ".";
            string pre = "\n";
            artical.Number = number;
            artical.ChapterID = chapterID;
            artical.ChapterItemID = chapterItemID;
            artical.ChapterNumber = ChapterNumber;
            //if (string.IsNullOrEmpty(ArticalContent))
            {
                artical.Content = Globals.NextItem(content, pre + artical.Name + tail);
                artical.Content = Globals.GetContentBetween(content, pre + artical.Name + tail, "Điều " + (number + 1) + tail, true, true);
            }
            if (artical.Content == "") return false;

            artical.Title = Globals.GetContentBetween(artical.Content, artical.Name + tail, pre + "1" + tail, false, true, "\r\n");

            command.CommandText = "exec GetArtical N'" + artical.Name + "', N'" + artical.Title + "',N'" + artical.Content + "'," + number + "," + chapterItemID + "," + chapterID;
            artical.ID = !isGenerateNewID ? -1 : Globals.GetIDinDT(db.ExecuteReaderCommand(command,""), 0, 0);

            if (artical.ID > 0 || !isGenerateNewID)
            {
                artical.lstClause = new List<Clause>();
                string TrimArticalContent = artical.Content.Substring(Math.Min(artical.Content.Length - 1, artical.Content.IndexOf(artical.Name + tail) + artical.Name.Length + 1));
                if (TrimArticalContent.IndexOf("\n1. ") > 0 && TrimArticalContent.IndexOf("\n2. ") > 0)
                {
                    Clause item = new Clause();
                    int ClauseNumber = 1;
                    while (ParseContentClause(command, isGenerateNewID, TrimArticalContent, ClauseNumber++, artical.ID))
                    {
                        artical.lstClause.Add(item); item = new Clause();
                    }
                }
                return true;
            }
            
            return false;
        }

        //Parse Clause
        public bool ParseContentClause(IDbCommand command, bool isGenerateNewID, string articalContent, int number, int ArticalID)
        {
            Clause clause = new Clause();
            clause.ArticalID = ArticalID;
            clause.Number = number;
            string Name = number + ".";
            // if (string.IsNullOrEmpty(Content))
            {
                clause.Content = Globals.NextItem(articalContent, "\n" + Name);
                clause.Content = Globals.GetContentBetween(clause.Content, "\n" + Name, (number + 1) + ".", true, true);
            }
            if (clause.Content == "") return false;
            bool hasLawPoint = clause.Content.IndexOf("a) ") > 0 && clause.Content.IndexOf("b) ") > 0;

            clause.Title = hasLawPoint ? Globals.GetContentBetween(clause.Content, "\n" + Name, "a)", true, true) : "";

            command.CommandText = "exec GetClause N'" + clause.Content + "', N'" + clause.Title + "'," + number + "," + ArticalID;
            clause.ID = !isGenerateNewID ? -1 : Globals.GetIDinDT(db.ExecuteReaderCommand(command,""), 0, 0);

            if (clause.ID > 0 || !isGenerateNewID)
            {
                clause.lstPoints = new List<Point>();
                string TrimLawPointContent = clause.Content.Substring(Math.Min(clause.Content.Length - 1, clause.Content.IndexOf(Name + ".") + Name.Length + 1));
                if (hasLawPoint)
                {
                    Point item = new Point();
                    int LawPointNumber = 1;
                    while (ParseContentPoint(command,isGenerateNewID, TrimLawPointContent, LawPointNumber++, clause.ID))
                    {
                        clause.lstPoints.Add(item); item = new Point();
                    }
                }
                return true;
            }

            return false;
        }

        //Parse Point
        private bool ParseContentPoint(IDbCommand command, bool isGenerateNewID, string clauseContent, int number, int ClauseID)
        {
            Point point = new Point();
            point.ClauseID = ClauseID;
            point.Number = number;
            string Name = point.GetPointName(number);
            if (Name == "") return false;
            string pre = "\n";
            //if (string.IsNullOrEmpty(Content))
            {
                point.Content = Globals.NextItem(clauseContent, pre + Name);
                point.Content = Globals.GetContentBetween(point.Content, pre + Name, pre + point.GetPointName(number + 1), true, true);
            }
            if (point.Content == "") return false;

            command.CommandText = "exec GetPoint N'" + point.Content + "', N'" + Name + "'," + number + "," + ClauseID;
            point.ID = !isGenerateNewID ? -1 : Globals.GetIDinDT(db.ExecuteReaderCommand(command,""), 0, 0);

            if (point.ID > 0 || !isGenerateNewID)
            {
                return true;
            }

            return false;
        }

        public void DeleteLawDocument(int id)
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
                command.CommandText = "select * from law where id = " + id;
                var rs = db.ExecuteReaderCommand(command,"");
                if(rs.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found",400,400);
                }

                command.CommandText = "exec DeleteLaw " + id;
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
                connection.Close();
            }
        }

        public void AddLawHTML(int LawID, string ContentHTML)
        {
            try
            {
                db.OpenConnection();

                var checkLawID = db.ExecuteReaderCommand($"select * from Law where id = {LawID}", "");            
                if(checkLawID.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found!", 400, 400);
                }
                string sql = $"  insert into LawHTML(LawID,ContentHTML)\r\n  values ({LawID},N'{ContentHTML}')";
                db.ExecuteNonQueryCommand(sql);
            }
            catch
            {
                throw;
            } finally
            {
                db.CloseConnection();
            }
        }

        public void UpdateLawHTML(int LawID, string ContentHTML)
        {
            try
            {
                db.OpenConnection();

                var checkLawID = db.ExecuteReaderCommand($"select * from Law where id = {LawID}", "");
                if (checkLawID.Rows.Count == 0)
                {
                    throw new BadRequestException("LawID not found!", 400, 400);
                }
                string sql = $"update LawHTML " +
                            $" set ContentHTML = N'{ContentHTML}' " +
                            $" where LawID = {LawID}";
                db.ExecuteNonQueryCommand(sql);
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

        public async Task AutoGenerateData(int LawID)
        {
            await keyPhraseService.GenerateKeyphraseVNCoreNLP(LawID);
            keyPhraseService.GenerateKeyphraseMapping(LawID);
            conceptService.GenerateConceptMapping(LawID);
        }
    }
}
