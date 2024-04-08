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
    public class SearchService : ISearchService
    {
        private readonly IDbService _db;

        public SearchService(IDbService db)
        {
            _db = db;
        }

        public List<ArticalResult> SearchLawByText(string searchInput)
        {
            try
            {
                _db.OpenConnection();
                SearchArticalResult rs = new SearchArticalResult();

                #region Get List KeyPhrase form input
                List<KeyPhrase> lstKeyPhrases_Searched = new List<KeyPhrase>();
                String sKey = Globals.GetKeyJoin(searchInput);
                string sqlGetKeyPhrases = "exec GetKeyPharesFromText N'" + sKey + "'";
                DataTable dtKeyPhrases = _db.ExecuteReaderCommand(sqlGetKeyPhrases, "");
                if(dtKeyPhrases.Rows != null && dtKeyPhrases.Rows.Count > 0)
                {
                    for (int i = 0; i < dtKeyPhrases.Rows.Count; i++)
                    {
                        lstKeyPhrases_Searched.Add(new KeyPhrase
                        {
                            ID = Globals.GetIDinDT(dtKeyPhrases, i, "ID"),
                            Key = Globals.GetinDT_String(dtKeyPhrases, i, "KeyPhrase"),
                            Source = (KeyPhraseSource)(Globals.GetIDinDT(dtKeyPhrases, i, "source")),
                            NumberArtical = Globals.GetIDinDT(dtKeyPhrases, i, "NumberArtical")
                        });
                    }
                }
                #endregion

                foreach (var item in lstKeyPhrases_Searched)
                {
                    item.Count = Globals.CountTerm(searchInput, item.Key.Replace("_", " "));
                }
                rs.KeyPhrases = lstKeyPhrases_Searched;

                #region Get List Concepts
                List<Concept> lstAllConcept = new List<Concept>();
                string sqlGetConcepts = "select * from [Concept]  with(nolock)";
                DataTable dtConcepts = _db.ExecuteReaderCommand(sqlGetConcepts, "");
                for (int i = 0; i < dtConcepts.Rows.Count; i++)
                {
                    lstAllConcept.Add(new Concept
                    {
                        ID = Globals.GetIDinDT(dtConcepts, i, "ID"),
                        Name = Globals.GetinDT_String(dtConcepts, i, "Name"),
                        Content = Globals.GetinDT_String(dtConcepts, i, "Description"),
                    });
                }
                #endregion

                List<KeyPhraseResult> lstCandidate_Concepts = new List<KeyPhraseResult>();
                for (int i = 0; i < lstAllConcept.Count; i++)
                {
                    lstCandidate_Concepts.Add(new KeyPhraseResult { 
                        ID = lstAllConcept[i].ID, 
                        keys = GetKeyPhrasesByConceptID(lstAllConcept[i].ID) 
                    });
                }

                int itopConcept = 3;
                int itopArticals = 10;
                double minScoreConcept = 0.01;
                double minScoreArtical = 0.01;

                rs.lstConcepts = TF_IDF.FindNearestNeighbors(lstCandidate_Concepts, lstKeyPhrases_Searched, minScoreConcept, itopConcept);
                
                // expand concepts
                List<KeyPhraseResult> lstCandidates = GetListArticalByConceptID(rs.lstConcepts.Select(x => x.ID).ToList());
                var show = rs.lstConcepts.Select(x => x.ID).ToList();
                rs.lstArticals = TF_IDF.FindNearestNeighbors(lstCandidates, lstKeyPhrases_Searched, minScoreArtical, itopArticals);

                foreach (var artical in rs.lstArticals)
                {
                    DataTable dsDetail = _db.ExecuteReaderCommand("exec GetArticalDetail2 " + artical.ID,"");
                    artical.Title = Globals.GetinDT_String(dsDetail, 0, "ChapterName") + " - " + Globals.GetinDT_String(dsDetail, 0, "Name");
                    artical.Content = Globals.GetinDT_String(dsDetail, 0, "Title");
                    artical.LawName = Globals.GetinDT_String(dsDetail, 0, "LawName");
                }

                List<ArticalResult> aRS = new List<ArticalResult>();
                foreach (var item in rs.lstArticals)
                {
                    aRS.Add(new ArticalResult
                    {
                        Title = item.Title,
                        Content = item.Content,
                        LawName = item.LawName,
                        ID = item.ID,
                        Distance = item.distance
                    });
                }

                return aRS;

            } catch
            {
                throw;
            }
            finally
            {
                _db.CloseConnection();
            }
        }

        private List<KeyPhrase> GetKeyPhrasesByConceptID(int conceptID)
        {
            List<KeyPhrase> lst = new List<KeyPhrase> ();
            DataTable dt = _db.ExecuteReaderCommand("exec GetKeyPharesByConceptID " + conceptID, "");
            string key = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                key = Globals.GetinDT_String(dt, i, "KeyPhrase");
                lst.Add(new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, i, "ID"),
                    Key = key,
                    Count = Globals.GetIDinDT(dt, i, "Count"),
                });
            }
            return lst;
        }

        private List<KeyPhraseResult> GetListArticalByConceptID(List<int> lst)
        {
            List<KeyPhraseResult> lstResult = new List<KeyPhraseResult>();
            DataTable dt = _db.ExecuteReaderCommand("Exec GetListArticalKeyPhraseByConceptID '" + string.Join(",", lst) + "'", "");
            var show = dt;
            SortedDictionary<int, List<KeyPhrase>> dic = new SortedDictionary<int, List<KeyPhrase>>();
            int articalID = 0; KeyPhrase ph;
            for (int i = 0; i < Globals.DTCount(dt); i++)
            {
                articalID = Globals.GetIDinDT(dt, i, "ArticalID");
                ph = new KeyPhrase
                {
                    ID = Globals.GetIDinDT(dt, i, "KeyPhraseID"),
                    Key = Globals.GetinDT_String(dt, i, "KeyPhrase"),
                    Count = Math.Max(1, Globals.GetIDinDT(dt, i, "NumCount")),
                };

                if (!dic.ContainsKey(articalID))
                {
                    dic.Add( articalID, new List<KeyPhrase> { ph } );
                }
                else
                {
                    dic[articalID].Add(ph);
                }

            }
            foreach (var item in dic.Keys)
            {
                lstResult.Add(new KeyPhraseResult { ID = item, keys = dic[item] });
            }
            return lstResult;
        }
    }
}
