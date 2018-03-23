using System.Collections.Generic;
using TrussMe.Model.Entities;

namespace TrussMe.Model.Interfaces
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        IEnumerable<ProjectTruss> GetProjectTruss(Project project);
        bool CheckCode(string projectId);
    }
}
