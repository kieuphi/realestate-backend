using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;
using General.Domain.Enums;

namespace General.Domain.Entities
{
    public class ConfigEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string ReceiveEmailContactUs { get; set; }
        public string ReceiveEmailBookShowing { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
