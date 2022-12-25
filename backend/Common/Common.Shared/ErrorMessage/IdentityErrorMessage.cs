using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.ErrorMessage
{
    public class IdentityErrorMessage
    {
        public const string PhoneNumberIsExist = "PhoneNumberIsExist";
        public const string ActivationCodeIsInvalid = "Activation code is invalid";
        public const string UserNameOrPasswordIncorrect = "UserNameOrPasswordIncorrect";
        public const string PasswordAndConfirmPasswordNotMatch = "Password and confirm password not match";
        public const string ExternalTokenIsInvalid = "Cannot get payload data. The external token is invalid";
        public const string ExternalLoginFailed = "External login failed.";
        public const string UserNotExist = "User is not exist on system.";
        public const string UserIsExist = "User is exist on system.";
        public const string EmailIsExist = "This email account is exist on system.";
        public const string InvalidCommunicationType = "Please using Email or SMS only.";
        public const string FailedToCreateOTP = "The OTP was failed to create. Please try again.";
        public const string InvalidToken = "The token is invalid or expired. Please contact Administrator.";
        public const string UserRoleIsNotExisted = "The user role is invalid.";
        public const string InternalUserHasExisted = "The user name or email or phone number is already existed.";
        public const string AccountWasVerified = "The account was verified";
        public const string ShareLinkIsNoteAvailable = "Share link is not available";
    }
}
