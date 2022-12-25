using Common.Shared.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.Localize
{
    public interface ILocalizationParser
    {
        Common.Shared.Models.Result ConvertResult(Common.Shared.Models.Result result);
        string ConvertText(string text);
    }
    public class LocalizationParser : ILocalizationParser
    {
        private readonly IStringLocalizer<Resource> _stringLocalizer;
        public LocalizationParser(IStringLocalizer<Resource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public Result ConvertResult(Result result)
        {
            for (int i = 0; i < result.Errors.Count; i++)
            {
                result.Errors[i] = _stringLocalizer[result.Errors[i]].Value;
            }
            return result;
        }
        public string ConvertText(string text)
        {
            return _stringLocalizer[text].Value;
        }
    }
}
