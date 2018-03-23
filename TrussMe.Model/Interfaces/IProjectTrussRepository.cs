using TrussMe.Model.Entities;

namespace TrussMe.Model.Interfaces
{
    public interface IProjectTrussRepository:IBaseRepository<ProjectTruss>
    {
        ProjectTruss GetInfo(Project project, global::TrussMe.Model.Entities.Truss truss);
        void SaveChanges(ProjectTruss projectTruss);
    }
}
