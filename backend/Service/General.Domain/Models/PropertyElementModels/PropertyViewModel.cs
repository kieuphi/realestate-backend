using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.PropertyElementModels
{
    public class PropertyViewModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public string ViewId { set; get; }

        public string viewVi { set; get; }
        public string viewEn { set; get; }
        public string ViewDescriptions { set; get; }
    }

    public class CreatePropertyViewModel
    {
        public string ViewId { set; get; }
    }
}
