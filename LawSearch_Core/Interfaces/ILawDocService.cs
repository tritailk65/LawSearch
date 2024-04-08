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
        List<LawDoc> GetListLawDoc();
        DataTable GetLawHTML(int ID);
        void ImportLaw(string name, string content);
        void DeleteLawDocument(int id);
        void AddLawHTML(int LawID , string ContentHTML);
        void UpdateLawHTML(int LawID, string ContentHTML);
    }
}
