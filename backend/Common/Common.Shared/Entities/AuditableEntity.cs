
using Common.Shared.Enums;
using System;

namespace Common.Shared.Entities
{
    public abstract class AuditableEntity
    {
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public DeletedStatus IsDeleted { get; set; }
        public int CurrentState { get; set; }
    }
}
