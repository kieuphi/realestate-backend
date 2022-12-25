using Common.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.DTOs
{
    public class AccountGroupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int NumberOfMenuAccess { get; set; }
        public ActiveStatus ActiveStatus { get; set; }
        public bool IsAdmin { get; set; }
        public List<AccountGroupMenuDTO> ListAccountGroupMenu { get; set; }
    }

    public class AccountGroupMenuDTO
    {
        public string MenuId { get; set; }
        public bool IsSubMenu { get; set; }
        public List<AccountGroupMenuDTO> ListAccountGroupMenu { get; set; }
        public bool IsView { get; set; }
    }
}
