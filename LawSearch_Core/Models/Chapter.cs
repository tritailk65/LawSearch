using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class Chapter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int LawID { get; set; }
        public List<Section> lstSection { get; set; }

        public Chapter() { lstSection = new List<Section>(); }

        internal string GetTextSignBeginArtical(string chapterContent)
        {
            string content = chapterContent;
            while (content.Length > 0)
            {
                int idex = content.IndexOf("Điều ");
                if (idex > 0)
                {
                    content = content.Substring(idex + 5);
                    string sign = ".";
                    int idex2 = content.IndexOf(sign);
                    if (idex2 < 0)
                    {
                        sign = ":";
                        idex2 = content.IndexOf(sign);
                    }
                    if (idex2 > 0)
                    {
                        string number = content.Substring(0, idex2);
                        int iNumber = 0;
                        if (int.TryParse(number, out iNumber))
                        {
                            return "Điều " + iNumber + sign;
                        }
                        content = content.Substring(idex2 + 1);
                    }
                }
                else return null;
            }
            return null;
        }
    }
}
