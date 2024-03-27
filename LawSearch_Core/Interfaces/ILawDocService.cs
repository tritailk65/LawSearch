using LawSearch_Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Interfaces
{
    public interface ILawDocService
    {
        DataTable GetListLawDoc();
        DataTable GetLawHTML(int ID);
        void ImportLaw(string name, string content);
    }
}
