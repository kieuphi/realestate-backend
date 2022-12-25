using System.Collections.Generic;

namespace General.Domain.Config
{
    public class AppSettings
    {
        public EmailConfiguration EmailConfiguration { get; set; }
        public Smtp Smtp { get; set; }
        public IncomAccount IncomAccount { get; set; }
        public PushNotificationAppInfo PushNotificationAppInfo { get; set; }
    }

    public class Smtp
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
    }

    public class EmailConfiguration
    {
        public string RecieveCompany { get; set; }
        public string RecieveDepartment { get; set; }
        public string RecieverEmailAddress { get; set; }
    }

    public class IncomAccount
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public TemplateCode TemplateCode { get; set; }
    }

    public class TemplateCode
    {
        public string OTP { get; set; }
    }

    public class PushNotificationAppInfo
    {
        public string BaseUrl { get; set; }
        public string AppId { get; set; }
        public string ApiKey { get; set; }
    }
}