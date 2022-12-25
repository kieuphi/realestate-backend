using General.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace General.Application.Interfaces
{
    public interface IHandlePropertyService
    {
        Task<string> GeneratePropertyNumber(string transactionType);
        Task<List<ListPropertyModel>> JoinPropertyElements(List<ListPropertyModel> list);
    }
}
