using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class TransactionTypeModel
    {
        public string Id { set; get; }
        public string Notation { set; get; }
        public string TransactionTypeVi { set; get; }
        public string TransactionTypeEn { set; get; }
        public List<PropertyTypeModel> PropertyTypes { set; get; }
    }
}
