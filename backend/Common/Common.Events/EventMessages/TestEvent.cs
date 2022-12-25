using System;

namespace Common.Events.EventMessages
{
    public class TestEvent : IntegrationBaseEvent
    {
        public string UserName { get; set; }
    }
}
