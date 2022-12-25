using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class MappingRoofStyle
    {
        public Guid Id { set; get; }
        public string RoofStyleId { set; get; }
        public Guid PropertyId { set; get; }
    }
}
