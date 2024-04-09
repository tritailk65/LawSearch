using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using static LawSearch_Admin.ViewModels.LawVM;

namespace LawSearch_Admin.Services
{
    public class LawDocAdminService : ILawDocAdminService
    {
        private readonly HttpClient _httpClient;

        public LawDocAdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LawDoc>> GetListLawDoc()
        {
            List<LawDoc> lst = new List<LawDoc>();
            var rs = await _httpClient.GetFromJsonAsync<APIResultVM<LawDoc>>($"api/lawdoc");
            if(rs != null && rs.Status == 200)
            {
                if (rs.Data.Count > 0)
                {
                    lst = rs.Data.ToList();
                }
            }
            return lst;
        }
        
        public async Task<LawHTML> GetLawHTML(int id)
        {
            LawHTML l = new LawHTML();
            var rs = await _httpClient.GetFromJsonAsync<APIResultVM<LawHTML>>($"api/lawdoc/GetLawHTML?id={id}");
            if(rs != null && rs.Status == 200 && rs.Data.Count > 0)
            {
                l = rs.Data[0];
            }

            return l;
        }

        public async Task<LawVM> GetDataLaw(int id)
        {
            LawVM lawVM = new LawVM();
            List<ChapterVM> lstChapters = new List<ChapterVM>();
            List<SectionVM> lstSection = new List<SectionVM>();
            List<ArticalVM> lstArtical = new List<ArticalVM>();

            //Load chapter
            var rsGetChapterByLawID = await _httpClient.GetFromJsonAsync<APIResultVM<ChapterVM>>($"api/Chapter/GetByLawID?ID={id}");
            if(rsGetChapterByLawID != null && rsGetChapterByLawID.Status == 200 && rsGetChapterByLawID.Data.Count > 0)
            {
                lstChapters = rsGetChapterByLawID.Data.ToList();
            }

            //Load section
            var rsGetSection = await _httpClient.GetFromJsonAsync<APIResultVM<SectionVM>>($"api/Section/GetByLawID?ID={id}");
            if (rsGetSection != null && rsGetSection.Status == 200 && rsGetSection.Data.Count > 0)
            {
                lstSection = rsGetSection.Data.ToList();
            }

            //Load artical
            var rsGetArtical = await _httpClient.GetFromJsonAsync<APIResultVM<ArticalVM>>($"api/Artical/GetByLawID?ID={id}");
            if (rsGetArtical != null && rsGetArtical.Status == 200 && rsGetArtical.Data.Count > 0)
            {
                lstArtical = rsGetArtical.Data.ToList();
            }

            var rs = lawVM.loadDataToView(lstChapters, lstSection, lstArtical);
            return rs;
        }

        public async Task<bool> ImportLaw(string name, string content)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", name),
                new KeyValuePair<string, string>("Content",content)
            });

            var rs = await _httpClient.PostAsync($"api/LawDoc", formContent);

            if (rs.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteLaw(int id)
        {
            var rs = await _httpClient.DeleteAsync($"api/LawDoc?ID={id}");
            if (rs.IsSuccessStatusCode)
            {
                return true;
            }else
            {
                return false;
            }
        }
    }
}
