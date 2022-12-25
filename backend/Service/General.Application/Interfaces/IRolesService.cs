using General.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Application.Interfaces
{
    public interface IRolesService
    {
        string GetRoleName(RoleTypes role);
    }
}
