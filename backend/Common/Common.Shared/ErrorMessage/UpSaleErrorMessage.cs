using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.ErrorMessage
{
    public class UpSaleErrorMessage
    {
        public const string TotalItemIsLargerThan20 = "The total subitems is larger than 20";
        public const string TheUpSaleRuleIsInvalid = "The upsale rule is invalid";
        public const string InvalidProductQuantityForUpSaleRule = "The product quantity is invalid for upsale rule.";
        public const string InvalidProductQuantityWithStoreSetting = "The product quantity is invalid with the store setting.";
    }
}
