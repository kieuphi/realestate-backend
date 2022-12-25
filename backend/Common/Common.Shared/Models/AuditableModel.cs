
using Common.Shared.Enums;
using System;

namespace Common.Shared.Models
{
    public abstract class AuditableModel
    {
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public DeletedStatus IsDeleted { get; set; }
        public int CurrentState { get; set; }
    }

    public abstract class CurrentStateModel
    {
        public int CurrentState { get; set; }
    }

    public abstract class ActiveStatusModel
    {
        public ActiveStatus ActiveStatus { get; set; }
    }

    public abstract class CommonModel
    {
        public ActiveStatus ActiveStatus { get; set; }
        public int CurrentState { get; set; }
    }
}
