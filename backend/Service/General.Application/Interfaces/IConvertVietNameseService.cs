using System;
using System.Collections.Generic;
using System.Text;

namespace General.Application.Interfaces
{
    public interface IConvertVietNameseService
    {
        string ConvertVietNamese(string text);
        string ConvertAdministrativedViToEn(string text, bool? isOnlyName = false);
    }
}
