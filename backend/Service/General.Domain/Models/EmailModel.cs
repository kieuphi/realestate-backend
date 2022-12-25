using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class EmailModel : ContentEmailModel
    {
        public string Sender { get; set; }
        public string EmailAddress { get; set; }
    }

    public class ConfirmationAccountModel
    {
        public string Email { set; get; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Identifier { get; set; }
        public string Token { get; set; }
    }

    public class ContentEmailModel : CommonEmailModel
    {
        public string MsgDearText { get; set; }
        public string MsgConcernText { get; set; }
        public string MsgPhoneContactText { get; set; }
        public string MsgEmailContactText { get; set; }
        public string MsgRemark { get; set; }
        public string MsgRegards { get; set; }
        public string MsgSignature { get; set; }

    }

    public class CommonEmailModel
    {
        public string SenderEmailAddress { get; set; }
        public string RecieverEmailAddress { get; set; }
        public string BCCEmailAddress { get; set; }
        public string CCEmailAddress { get; set; }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserRole { get; set; }
    }

    public class ContactUsEmailModel
    {
        public string Email { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerMessage { get; set; }
        public string Subject { set; get; }
    }

    public class BookShowingEmailModel
    {
        public string SellerEmail { set; get; }
        public List<string> ListEmailCC { get; set; }
        public string SellerName { get; set; }
        public string ContactType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerMessage { get; set; }
        public string PropertyNumber { get; set; }
        public string PropertyTitle { get; set; }
        public string TransactionType { get; set; }
        public string PropertyType { get; set; }
        public string Subject { set; get; }

        public string ProjectName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
}
