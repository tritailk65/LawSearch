using LawSearch_Core.Models;
using System.Collections.Generic;
using System.Net;

namespace LawSearch_Admin.ViewModels
{
    public class LawVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<ChapterVM> lstChapters { get; set; }

        public LawVM()
        { 

        }

        public LawVM loadDataToView(List<ChapterVM> lstChapters, List<SectionVM> lstSections, List<ArticalVM> lstArticals)
        {
            LawVM lawVM = new LawVM();

            foreach (var c in lstChapters)
            {
                List<SectionVM> section = (from i in lstSections
                                         where i.ChapterID == c.ID
                                         select i).ToList();
                c.lstSections = section;
                foreach (var s in c.lstSections)
                {
                    List<ArticalVM> articals = (from i in lstArticals
                                              where i.ChapterItemID == s.ID
                                              select i).ToList();
                    s.lstArticals = articals;
                }
            }
            lawVM.lstChapters = lstChapters;
            return lawVM;
        }

        public class ChapterVM
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public List<SectionVM> lstSections { get; set; }
            public List<ArticalVM> lstArticals { get; set; }
        }

        public class SectionVM
        {
            public int ID { get; set; }
            public int ChapterID { get; set; }
            public string Name { get; set; }
            public List<ArticalVM> lstArticals { get; set; }
        }

        public class ArticalVM
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int ChapterItemID { get; set; }
            public int ChapterID { get; set; }
        }
        
        public class ClauseVM
        {
            public int ID { get; set; }

        }
    }
}
