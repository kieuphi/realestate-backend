using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.ErrorMessage
{
    public class VnpayMessageDefine
    {
        public const string ConfirmSuccess = "Confirm success";
        public const string OrderAlreadyConfirmed = "Order already confirmed";
        public const string InvalidAmount = "Invalid amount";
        public const string OrderNotFound = "Order not found";
        public const string InvalidSignature = "Invalid signature";
        public const string InputDataRequired = "Input data required";
        public const string PurchaseOrderWasPaidOrCanceled = "The purchase order was paid or canceled";
        public const string InvalidShippingInformation = "Invalid shipping information";
        public const string InvalidCustomerInformation = "Invalid customer information";
        public const string CannotGetUserProfile = "Cannot get user profile";
        public const string UnknownError = "Unknown error";
    }

    public class VnpayCodeDefine
    {
        public const string Code00 = "00";
        public const string Code01 = "01";
        public const string Code02 = "02";
        public const string Code03 = "04";
        public const string Code04 = "04";
        public const string Code97 = "97";
        public const string Code99 = "99";
    }
}
