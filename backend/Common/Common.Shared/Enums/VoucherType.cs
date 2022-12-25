using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.Enums
{
    public enum VoucherType
    {
        FixedOnOrderAmount = 1,
        FixedOnShippingAmount = 2,
        DiscountOnOrderAmount = 3, 
        DiscountOnShippingAmount = 4
    }
}
