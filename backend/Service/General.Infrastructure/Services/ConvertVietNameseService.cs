using General.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace General.Infrastructure.Services
{
    public class ConvertVietNameseService : IConvertVietNameseService
    {
        private static readonly string[] VietNamChar = new string[] {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public string ConvertVietNamese(string text)
        {
            if(!string.IsNullOrEmpty(text))
            {
                for (int i = 1; i < VietNamChar.Length; i++)
                {
                    for (int j = 0; j < VietNamChar[i].Length; j++)
                    {
                        text = text.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
                    }
                }

                return text;
            }
            else
            {
                return "";
            }
        }

        public string ConvertAdministrativedViToEn(string text, bool? isOnlyName = false)
        {
            string vietnameseString = ConvertVietNamese(!string.IsNullOrEmpty(text) ? text : "");
            string result = vietnameseString;

            if (vietnameseString.Length > 0)
            {
                // thành phố - city
                if (vietnameseString.ToLower().StartsWith(("thanh pho")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "thanh pho", "city", isOnlyName);
                }
                // tỉnh - province
                else if (vietnameseString.ToLower().StartsWith(("tinh")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "tinh", "province", isOnlyName);
                }
                // quận - district
                else if (vietnameseString.ToLower().StartsWith(("quan")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "quan", "district");
                }
                // huyện - district
                else if (vietnameseString.ToLower().StartsWith(("huyen")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "huyen", "district");
                }
                // phường - ward
                else if (vietnameseString.ToLower().StartsWith(("phuong")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "phuong", "ward");
                }
                // thị xã - commune
                else if (vietnameseString.ToLower().StartsWith(("thi xa")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "thi xa", "commune");
                }
                // xã - commune
                else if (vietnameseString.ToLower().StartsWith(("xa")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "xa", "commune");
                }
                // thị trấn - town
                else if (vietnameseString.ToLower().StartsWith(("thi tran")) == true)
                {
                    result = HandleAdministrativedViToEn(vietnameseString, "thi tran", "town");
                }
            }

            return result;
        }

        private string HandleAdministrativedViToEn(string fullText, string viKeyword, string enKeyword, bool? isOnlyName = false)
        {
            string result = "";
            if (!string.IsNullOrEmpty(fullText))
            {
                string removeViCharacters = fullText.ToLower().Remove(0, viKeyword.Length).Trim();
                if (Regex.IsMatch(removeViCharacters.Substring(0, 1), @"^\d+$") == true)
                {
                    result = isOnlyName == false ? enKeyword + " " + removeViCharacters : removeViCharacters;
                }
                else
                {
                    result = isOnlyName == false  ? removeViCharacters + " " + enKeyword : removeViCharacters;
                }
            }
            else
            {
                result = "";
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result).Trim();
        }
    }
}
