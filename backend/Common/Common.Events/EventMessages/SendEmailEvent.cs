namespace Common.Events.EventMessages
{
    public class SendEmailEvent : IntegrationBaseEvent
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
