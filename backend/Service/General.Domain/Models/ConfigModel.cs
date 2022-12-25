using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class ConfigModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string ReceiveEmailContactUs { get; set; }
        public string ReceiveEmailBookShowing { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public List<string> ListReceiveEmailContactUs { get; set; }
        public List<string> ListReceiveEmailBookShowing { get; set; }
    }

    public class CreateConfigModel
    {
        public string ReceiveEmailContactUs { get; set; }
        public string ReceiveEmailBookShowing { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
