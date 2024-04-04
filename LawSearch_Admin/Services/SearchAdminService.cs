﻿using LawSearch_Admin.Interfaces;
using LawSearch_Admin.ViewModels;
using LawSearch_Core.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace LawSearch_Admin.Services
{
    public class SearchAdminService : ISearchAdminService
    {
        private readonly HttpClient httpClient;

        public SearchAdminService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<ArticalResult>> GetResultSearchLaw(string input)
        {
            List<ArticalResult> lst = new List<ArticalResult>();

            var rs = await httpClient.GetFromJsonAsync<APIResultVM<ArticalResult>>($"api/Search?input={input}");

            if (rs != null && rs.Status == 200 && rs.Data != null)
            {
                lst = rs.Data;
            }
            return lst;
        }
    }
}