using Common.Shared.Entities;
using Common.Shared.Enums;
using System;

namespace General.Domain.Entities
{
    public class ActiveCodeInformationEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string ActivationCode { get; set; }
        public ActivationType ActivationType { get; set; }
        public DateTime ExpiredTime { get; set; }
        public ActiveStatus Status { get; set; }
    }
}
