using LawSearch_Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Section
    {
        public int LawID { get; set; }
        public int ID { get; set; }
        public int ChapterID { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ChapterNumber { get; set; }
        public List<Artical> lstArtical { get; set; }

        internal int GetFirstArticalNumber(string chapterItemContent, int chapterItemnumber)
        {

            if (chapterItemnumber == 0)
            {
                if (chapterItemContent.IndexOf("Điều ", StringComparison.OrdinalIgnoreCase) < 0)
                    return -1;
            }
            else if (chapterItemContent.IndexOf("MỤC " + chapterItemnumber + ". ", StringComparison.OrdinalIgnoreCase) < 0 && chapterItemContent.IndexOf("MỤC " + chapterItemnumber + ":", StringComparison.OrdinalIgnoreCase) < 0)
                return -1;

            string sNumber = Globals.GetContentBetween(chapterItemContent, "\nĐiều ", ".", true, true);
            if (sNumber == "")
                sNumber = Globals.GetContentBetween(chapterItemContent, "\nĐiều ", ":", true, true);
            if (sNumber != "")
            {
                if (sNumber.Contains("\n"))
                    sNumber = sNumber.Substring(sNumber.LastIndexOf("\n"));
                sNumber = sNumber.Replace("Điều ", "").Replace(".", "").Replace(":", "").Trim();
                int iNumber = 0;
                return int.TryParse(sNumber, out iNumber) ? iNumber : -1;
            }
            return -1;
        }
    }
}
