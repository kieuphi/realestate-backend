using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.PropertyElementModels
{
    public class PropertyAmenitiesNearbyModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public string AmenitiesNearbyId { set; get; }

        public string AmenitiesNearbyVi { set; get; }
        public string AmenitiesNearbyEn { set; get; }
        public string Descriptions { set; get; }
    }

    public class CreatePropertyAmenitiesNearbyModel
    {
        public string AmenitiesNearbyId { set; get; }
    }
}
