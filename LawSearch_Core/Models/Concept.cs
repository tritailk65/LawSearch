using LawSearch_Core.Common;
using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Concept
    {
        public int ID { get; set; }
        public string Name { get; set; }    
        public string Content { get; set; }
        public int type { get; set; }
        public double[] vector;
        public double distance;
    }

    public class AddConceptKeyphrase
    {
        public int ConceptID { get; set; }
        public string Keyphrase { get; set; }
    }

    public class ConceptMapping
    {
        public int ID { get; set; }
        public int ConceptID { get; set; }
        public int ArticalID { get; set; }
        public int ChapterID { get; set; }
        public int ChapterItemID { get; set; }
        public int ClauseID { get; set; }
        public int PointID { get; set; }
        public int LawID { get; set; }

        public ConceptMapping(int conceptID, int articalID, int chapterID, int chapterItemID, int clauseID, int pointID, int lawID)
        {
            ConceptID = conceptID;
            ArticalID = articalID;
            ChapterID = chapterID;
            ChapterItemID = chapterItemID;
            ClauseID = clauseID;
            PointID = pointID;
            LawID = lawID;
        }
    }
}
