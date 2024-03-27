using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class FileDetails
    {
        public string FileName { get; set; }
        public int Size { get; set; }
        public byte[] FileData { get; set; }
        public FileType fileType { get; set; }

        public enum FileType
        {
            PDF = 1,
            DOCX = 2,
        }
    }
}
