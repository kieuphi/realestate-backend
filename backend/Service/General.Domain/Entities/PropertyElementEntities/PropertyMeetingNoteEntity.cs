using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.PropertyElementEntities
{
    public class PropertyMeetingNoteEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string MeetingNoteTitle { set; get; }
        public string MeetingNoteContent { set; get; }
        public Guid PropertyId { set; get; }
    }
}
