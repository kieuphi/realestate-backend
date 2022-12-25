using General.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        public ResetPasswordService(string baseUrl, string resetPasswordInternalUserUrl)
        {
            ResetPasswordUrl = baseUrl;
            ResetPasswordInternalUserUrl = resetPasswordInternalUserUrl;
        }
        public string ResetPasswordUrl { get; set; }
        public string ResetPasswordInternalUserUrl { get; set; }
    }
}
