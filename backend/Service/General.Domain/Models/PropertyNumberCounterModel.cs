using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class PropertyNumberCounterModel
    {
        public Guid Id { set; get; }
        public DateTime Date { set; get; }
        public string TransactionType { set; get; }
        public int? CurValue { set; get; }
    }

    public class CreatePropertyNumberCounterModel
    {
        public string TransactionType { set; get; }
    }
}
