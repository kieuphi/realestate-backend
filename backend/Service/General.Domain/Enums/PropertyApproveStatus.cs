using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Enums
{
    public enum PropertyApproveStatus
    {
        New = 1,
        Active = 2,
        InActive = 3,
        Lock = -1,
        Expired = 4,
        Draft = 5
    }
}