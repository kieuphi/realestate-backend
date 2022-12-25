using System;
using System.Collections.Generic;
using System.Text;

namespace General.Application.Interfaces
{
    public interface IResetPasswordService
    {
        string ResetPasswordUrl { get; }
        public string ResetPasswordInternalUserUrl { get; }
    }
}
