using General.Application.Interfaces;
using General.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Services
{
    public class RolesService : IRolesService
    {
        public string GetRoleName(RoleTypes role)
        {
            string roleName = "";

            if (role == RoleTypes.SystemAdministrator)
            {
                roleName = "SystemAdministrator";
            }
            else if (role == RoleTypes.InternalUser)
            {
                roleName = "InternalUser";
            }
            else if (role == RoleTypes.LocalService)
            {
                roleName = "LocalService";
            }
            else if (role == RoleTypes.Seller)
            {
                roleName = "Seller";
            }
            else if (role == RoleTypes.Guest)
            {
                roleName = "Guest";
            }
            //else if (role == RoleTypes.Supplier)
            //{
            //    roleName = "Supplier";
            //}

            return roleName;
        }
    }
}
