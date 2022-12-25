using System;
using System.Collections.Generic;
using General.Domain.Enums;

namespace General.Application.Common.Results
{
    public class IdentityResult
    {
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public UserStatus UserStatus { get; set; }
        public IdentityResult() {}

        internal IdentityResult(
           bool succeeded,
           string userName,
           IList<string> roles,
           string email,
           string accessToken,
           string refreshToken,
           UserStatus userStatus = UserStatus.LoginFailed,
           string errorMessage = "")
        {
            Succeeded = succeeded;
            UserName = userName;
            Roles = roles;
            RefreshToken = refreshToken;
            AccessToken = accessToken;
            Email = email;
            ErrorMessage = errorMessage;
            UserStatus = userStatus;
        }

        internal IdentityResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            ErrorMessage = error;
            UserStatus = UserStatus.LoginSucceeded;
        }

        internal IdentityResult(bool succeeded, string error, UserStatus userStatus)
        {
            Succeeded = succeeded;
            ErrorMessage = error;
            UserStatus = userStatus;
        }

        public static IdentityResult Success(
            string userName,
            IList<string> roles,
            string email,
            string accessToken,
            string refreshToken)
        {
            return new IdentityResult(true, userName, roles, email, accessToken, refreshToken, UserStatus.LoginSucceeded);
        }

        public static IdentityResult Error(string error)
        {
            return new IdentityResult(false, error, UserStatus.LoginFailed);
        }

        public static IdentityResult LockedOut(string error = "User has been locked. Please contact Administrators for unlock this user")
        {
            return new IdentityResult(false, error, UserStatus.LockedUser);
        }

        public static IdentityResult MustChangePassword(string error = "Please change password for first time login")
        {
            return new IdentityResult(false, error, UserStatus.MustChangePassword);
        }
    }
}
