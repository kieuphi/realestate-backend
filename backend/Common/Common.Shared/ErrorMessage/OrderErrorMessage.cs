using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.ErrorMessage
{
    public class OrderErrorMessage
    {
        public const string CustomerIdInvalid = "CustomerIdInvalid";
        public const string ShippingAddressInvalid = "ShippingAddressInvalid";
        public const string TransactionIdInvalid = "TransactionId invalid";
        public const string PaymentMethodIdInvalid = "PaymentMethodIdInvalid";
        public const string PaymentMethodIsNull = "Payment Method is invalid";
        public const string ComboNumberInvalid = "ComboNumberInvalid";
        public const string OrderQuantityInvalid = "OrderQuantityInvalid";
        public const string MainProductInvalid = "MainProductInvalid";
        public const string MainProductNotFound = "MainProductNotFound";
        public const string SubProductNotFound = "SubProductNotFound";
        public const string ProductNotExistOrDelete = "ProductNotExistOrDelete";
        public const string ComboCodeNotFound = "Combo code not found";
        public const string SalePriceInvalid = "SalePriceInvalid";
        public const string BestSalePriceInvalid = "BestSalePriceInvalid";
        public const string VATInvalid = "VATInvalid";
        public const string DiscountInvalid = "DiscountInvalid";
        public const string ShippingCostInvalid = "ShippingCostInvalid";
        public const string VoucherInvalid = "VoucherInvalid";
        public const string VoucherDiscountAmountInvalid = "The voucher discount amount is invalid";
        public const string AddressInfoNotFound = "AddressInfoNotFound";
        public const string SaveOrderFailed = "Save order failed";
        public const string MultipleProductSameTypeInCombo = "Cannot create order with multiple products same type in a combo";
        public const string CannotGetPOCode = "Cannot get PO code";
        public const string StoreSettingInvalid = "StoreSettingInvalid";
        public const string StoreSettingProductInsufficient = "StoreSettingProductInsufficient";
        public const string StoreSettingProductNotActive = "StoreSettingProductNotActive";
        public const string StoreSettingUpdateQuantityFailed = "StoreSettingUpdateQuantityFailed";
        public const string CannotChangeOrderStatusWasCompletedOrCanceled = "Cannot change purchase order status that was completed or canceled";
        public const string NewStatusInvalid = "New status invalid";
        public const string CannotGetComboCode = "Cannot get combo code";
        public const string TotalItemIsLargerThan20 = "The total subitems is larger than 20";
        public const string ShippingMethodInvalid = "ShippingMethodInvalid";
        public const string ShippingFeeInvalid = "ShippingFeeInvalid";
    }
}
