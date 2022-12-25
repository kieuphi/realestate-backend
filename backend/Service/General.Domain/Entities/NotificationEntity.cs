using Common.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class NotificationEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TitleVi { get; set; }
        public string Content { get; set; }
        public string ContentVi { get; set; }
        public string Link { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedTime { get; set; }
        public string PostedBy { get; set; }
    }

    public class UserNotificationRemoveEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
    }

    public class UserNotificationSeenEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NotificationId { get; set; }
    }
}
