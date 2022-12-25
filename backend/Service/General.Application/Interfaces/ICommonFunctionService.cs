using System;
using System.Collections.Generic;
using System.Text;

namespace General.Application.Interfaces
{
    public interface ICommonFunctionService
    {
        string ConvertImageUrl(string imageUrl);
        string GenerateFriendlyUrl(string keyword, int count);
    }
}
