﻿using LawSearch_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Common
{
    public class Globals
    {

        //Hàm lấy giá trị cột ID theo tên v
        public static int GetIDinDT(DataTable dt, int row, string v)
        {
            return dt == null || dt.Rows.Count == 0 || dt.Rows[row][v].ToString() == "" ? -1 : int.Parse(dt.Rows[row][v].ToString() ?? "");
        }

        //Hàm lấy giá trị cột ID theo vị trí cột v
        public static int GetIDinDT(DataTable dt, int row, int v)
        {
            return dt == null || dt.Rows.Count == 0 ? -1 : int.Parse(dt.Rows[row][v].ToString() ?? "");
        }

        //Lấy giá trị cột v
        public static string GetinDT_String(DataTable dt, int row, string v)
        {
            return dt == null || dt.Rows.Count == 0 ? "" : dt.Rows[row][v].ToString() ?? "";
        }

        //Hàm lấy giá trị tại vị trí tương ứng
        public static string GetinDT_String(DataTable dt, int row, int v)
        {
            return dt == null || dt.Rows.Count == 0 ? "" : dt.Rows[row][v].ToString() ?? "";
        }

        //Hàm đếm số lượng row trong table
        public static int DTCount(DataTable dsKey)
        {
            return dsKey == null || dsKey.Rows.Count == 0 ? 0 : dsKey.Rows.Count;
        }

        //Loai bo khoang trong thay bang dau "_"
        internal static string GetKeyJoin(string searchInput)
        {
            return string.Join("_", searchInput.Split(' ').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }

        internal static int CountTerm(string inputtext, string searchTerm)
        {
            string text = NormalizeVietnamese(inputtext);
            searchTerm = NormalizeVietnamese(searchTerm);
            if (!text.Contains(searchTerm)) return 0;
            var arr = text.Split(new string[] { searchTerm }, StringSplitOptions.RemoveEmptyEntries);
            int count = arr.ToList().Where(x => x != "").Count();
            return Math.Max(1, count - 1);
        }

        private static string NormalizeVietnamese(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD)).ToLower();
        }

        //Get ký tự la mã
        internal static string GetLama(int _Number)
        {
            try
            {
                string strRet = string.Empty;
                bool _Flag = true;
                string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
                int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
                int i = 0;
                while (_Flag)
                {
                    while (_Number >= ArrNumber[i])
                    {
                        _Number -= ArrNumber[i];
                        strRet += ArrLama[i];
                        if (_Number < 1)
                            _Flag = false;
                    }
                    i++;
                }
                return strRet;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        internal static string NextItem(string chuongContent, string v)
        {
            int idx = chuongContent.IndexOf(v, StringComparison.CurrentCultureIgnoreCase);
            return idx >= 0 ? chuongContent.Substring(idx) : "";
        }

        internal static string GetContentBetween(string content, string begin, string end, bool isKeepBegin, bool isCleanBreakLine, string secondEnd = "", string thirdEnd = "")
        {
            if (content == "" || content.IndexOf(begin, StringComparison.CurrentCultureIgnoreCase) < 0) return "";
            string data = content.Substring(content.IndexOf(begin, StringComparison.CurrentCultureIgnoreCase));
            if (!isKeepBegin)
                data = data.Substring(begin.Length);
            if (data.IndexOf(end, StringComparison.CurrentCultureIgnoreCase) >= 0)
                data = data.Substring(0, data.IndexOf(end, StringComparison.CurrentCultureIgnoreCase));
            else
            {
                if (secondEnd != "" && data.IndexOf(secondEnd, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    data = data.Substring(0, data.IndexOf(secondEnd, StringComparison.CurrentCultureIgnoreCase));
                else if (thirdEnd != "" && data.IndexOf(thirdEnd, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    data = data.Substring(0, data.IndexOf(thirdEnd, StringComparison.CurrentCultureIgnoreCase));

            }
            if (isCleanBreakLine)
                data = data.TrimEnd('\r').TrimEnd('\n').Trim();
            return data;

        }

        internal static string GetTail(string content, string Name)
        {
            return content == null || Name == null ? "" : content.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && content.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0 ? ":" : ".";
        }
    }
}
