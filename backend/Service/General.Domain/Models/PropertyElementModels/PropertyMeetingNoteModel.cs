using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.PropertyElementModels
{
    public class PropertyMeetingNoteModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string MeetingNoteTitle { set; get; }
        public string MeetingNoteContent { set; get; }
        public Guid PropertyId { set; get; }
    }

    public class CreatePropertyMeetingNoteModel
    {
        public string MeetingNoteTitle { set; get; }
        public string MeetingNoteContent { set; get; }
    }
}
