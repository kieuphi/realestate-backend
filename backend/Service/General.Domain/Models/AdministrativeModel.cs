using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class AdministrativeModel
    {
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }

        public string CoordinatesProvince { set; get; }
        public string CoordinatesDistrict { set; get; }
        public string CoordinatesWard { set; get; }
    }
}
