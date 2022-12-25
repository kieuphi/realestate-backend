using General.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace General.Application.Interfaces
{
    public interface IHandleProjectService
    {
        Task<List<ProjectModel>> JoinProjectElements(List<ProjectModel> list);
    }
}
