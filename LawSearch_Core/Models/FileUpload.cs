using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LawSearch_Core.Models.FileDetails;

namespace LawSearch_Core.Models
{
    public class FileUpload
    {
        public IFormFile File { get; set; }
        public FileType FileType { get; set; }
    }
}
