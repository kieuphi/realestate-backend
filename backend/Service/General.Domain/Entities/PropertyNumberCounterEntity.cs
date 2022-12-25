using System;

namespace General.Domain.Entities
{
    public class PropertyNumberCounterEntity
    {
        public Guid Id { set; get; }
        public DateTime Date { set; get; }
        public string TransactionType { set; get; }
        public int? CurValue { set; get; }
    }
}
