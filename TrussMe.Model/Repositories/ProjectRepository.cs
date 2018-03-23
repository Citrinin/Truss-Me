using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;
using ProjectDA = TrussMe.DataAccess.Entities.Project;
using TrussDA = TrussMe.DataAccess.Entities.Truss;
//using BarDA = TrussMe.DataAccess.Entities.Bar;
//using SectionDA = TrussMe.DataAccess.Entities.Section;
//using SteelDA = TrussMe.DataAccess.Entities.Steel;
//using TrussElementDA = TrussMe.DataAccess.Entities.TrussElement;
//using TypeOfLoadDA = TrussMe.DataAccess.Entities.TypeOfLoad;

namespace TrussMe.Model.Repositories
{

    public class ProjectRepository : IProjectRepository
    {

        private readonly TrussContext _trussContext;

        public ProjectRepository()
        {
            this._trussContext = new TrussContext();
            ModelMappingProfile.MapperInitializer(this._trussContext);

        }
        public void Add(Project item)
        {
            using (var trussContext= new TrussContext())
            {
                var projDbMap = Mapper.Map<ProjectDA>(item);
                trussContext.Project.Add(projDbMap);
                trussContext.SaveChanges();
                item.ProjectId = projDbMap.ProjectId;
            }

        }

        public bool CheckCode(string projectCode)
        {
            return _trussContext.Project.ToArray().FirstOrDefault(proj=>proj.Code == projectCode)==null;
        }

        public IEnumerable<Project> GetAll()
        {
            return _trussContext.Project.ToArray().Select(Mapper.Map<Project>);
        }

        public IEnumerable<ProjectTruss> GetProjectTruss(Project project)
        {
            return _trussContext.
                ProjectTruss.Where(projtr=>projtr.ProjectId==project.ProjectId).
                ToArray().Select(Mapper.Map<ProjectTruss>);
        }

        public void Remove(Project itemToDelete)
        {
            var prTrusstoDelete = new List<DataAccess.Entities.ProjectTruss>(_trussContext.ProjectTruss.Where(prtr => prtr.ProjectId == itemToDelete.ProjectId));
            var trussestoDelete = new List<TrussDA>( prTrusstoDelete.Select(prtr => prtr.Truss).Where(tr => !tr.UnitForce));

            foreach (var item in trussestoDelete)
            {
                var bars = from bar in _trussContext.Bar
                           where bar.TrussId == item.TrussId
                           select bar;
                _trussContext.Bar.RemoveRange(bars.ToList());
            }
            //_trussContext.Truss.RemoveRange(trussestoDelete);
            _trussContext.ProjectTruss.RemoveRange(prTrusstoDelete);
            _trussContext.Truss.RemoveRange(trussestoDelete);
            _trussContext.Project.Remove(_trussContext.Project.First(pr=>pr.ProjectId==itemToDelete.ProjectId));
            _trussContext.SaveChanges();
        }

        public void Update(Project item)
        {
            var projectToUpdate = _trussContext.Project.First(pr=>pr.ProjectId==item.ProjectId);
            projectToUpdate.Code = item.Code;
            projectToUpdate.Description = item.Description;
            _trussContext.SaveChanges();
        }
         
    }
}
