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

/*        public List<ChapterVM> loadChapter()
        {
            List<ChapterVM> lstChapters = new List<ChapterVM>
            {
                new ChapterVM
                {
                    ID = 1,
                    Name = "Chương 1",
                    Title = "Quy hoạch, kế hoạch sử dụng đất"
                },
                new ChapterVM
                {
                    ID = 2,
                    Name = "Chương 2",
                    Title = "Giao đất, cho thuê đất, chuyển mục đích sử dụng đất"
                }
            };
            return lstChapters;
        }

        public List<SectionVM> loadSection()
        {
            List<SectionVM> lstSections = new List<SectionVM>
            {
                new SectionVM
                {
                    ID = 1,
                    ChapterID = 1,
                    Name = "Mục 1"
                }
            };
            return lstSections;
        }

        public List<ArticalVM> loadArtical()
        {
            List<ArticalVM> lstArticals = new List<ArticalVM>
            {
                new ArticalVM
                {
                    ID = 1,
                    Name = "Điều 1",
                    Title = "Nhà nước quyết định giá đất",
                    ChapterID = 1,
                    ChapterItemID = 1,
                    Content = "  Điều 15. Nhà nước quy định hạn mức sử dụng đất, thời hạn sử dụng đất\r\n\r\n1. Nhà nước quy định hạn mức sử dụng đất gồm hạn mức giao đất nông nghiệp, hạn mức giao đất ở, hạn mức công nhận quyền sử dụng đất ở và hạn mức nhận chuyển quyền sử dụng đất nông nghiệp.\r\n\r\n2. Nhà nước quy định thời hạn sử dụng đất bằng các hình thức sau đây:\r\n\r\na) Sử dụng đất ổn định lâu dài;\r\n\r\nb) Sử dụng đất có thời hạn."
                },
                new ArticalVM
                {
                    ID = 2,
                    Name = "Điều 2",
                    Title = "Nguyên tắc sử dụng đất",
                    ChapterID = 1,
                    ChapterItemID = 1,
                },
                new ArticalVM
                {
                    ID = 3,
                    Name = "Điều 3",
                    Title = "Căn cứ để xác định loại đất",
                    ChapterID = 1,
                    ChapterItemID = 1,
                    Content = "  Điều 9. Khuyến khích đầu tư vào đất đai\r\n\r\nNhà nước có chính sách khuyến khích người sử dụng đất đầu tư lao động, vật tư tiền vốn và áp dụng thành tựu khoa học, công nghệ vào các việc sau đây:\r\n\r\n1. Bảo vệ, cải tạo, làm tăng độ màu mỡ của đất;\r\n\r\n2. Khai hoang, phục hóa, lấn biển, đưa diện tích đất trống, đồi núi trọc, đất có mặt nước hoang hóa vào sử dụng theo quy hoạch, kế hoạch sử dụng đất;\r\n\r\n3. Phát triển kết cấu hạ tầng để làm tăng giá trị của đất.",
                }
            };
            return lstArticals;
        }*/
/*
        public List<Clause> loadClause()
        {
            lstClause = new List<Clause>
            {
                new Clause
                {
                    ID = 1,
                    Number = 1,
                    ArticalID = 1,
                },
                new Clause
                {
                    ID = 2,
                    Number = 2,
                    ArticalID = 1,
                },
                new Clause
                {
                    ID = 3,
                    Number = 3,
                    ArticalID = 1,
                }
            };
            return lstClause;
        }

        public List<Point> loadPoint()
        {
            lstPoints = new List<Point>
            {
                new Point
                {
                    ID = 1,
                    ClauseID = 1,
                    Name = "a)"
                },
                new Point
                {
                    ID = 2,
                    ClauseID = 1,
                    Name = "b)"
                }
            };
            return lstPoints;
        }*/
    }
}
