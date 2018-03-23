using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;

namespace TrussMe.Model.Repositories
{
    public class ProjectTrussRepository : IProjectTrussRepository
    {

        private readonly TrussContext _trussContext;

        public ProjectTrussRepository(TrussContext trussContext)
        {
            this._trussContext = trussContext;
        }

        public ProjectTruss GetInfo(Project project, Truss truss)
        {
            var pt = _trussContext.ProjectTruss
                .ToArray()
                .FirstOrDefault(projTruss => projTruss.ProjectId == project.ProjectId && projTruss.TrussId == truss.TrussId);
            return pt != null ? Mapper.Map<ProjectTruss>(pt) : null;
        }

        public void SaveChanges(ProjectTruss projectTruss)
        {
            var projectTrussesToChange = _trussContext.ProjectTruss.First(ptr =>
                ptr.ProjectId == projectTruss.ProjectId &&
                ptr.TrussId == projectTruss.TrussId &&
                ptr.TrussName == projectTruss.TrussName);

            projectTrussesToChange.Load = projectTruss.Load;
            projectTrussesToChange.TrussSpacing = projectTrussesToChange.TrussSpacing;
            var updatedTruss = _trussContext.Truss.First(tr => tr.TrussId == projectTrussesToChange.TrussId);
            if (!updatedTruss.UnitForce)
            {
                foreach (var newBar in updatedTruss.Bar)
                {
                    var bar = projectTruss.Truss.Bar.First(br => br.BarId == newBar.BarId);
                    newBar.Force = bar.ActualForce;
                    newBar.Moment = bar.ActualMoment;
                    newBar.Length = bar.Length;
                    newBar.SectionId = bar.SectionId;
                    newBar.SteelId = bar.SteelId;
                }
            }
            else
            {
                foreach (var newBar in updatedTruss.Bar)
                {
                    var bar = projectTruss.Truss.Bar.First(br => br.BarId == newBar.BarId);
                    newBar.SectionId = bar.SectionId;
                    newBar.SteelId = bar.SteelId;
                }
            }

            _trussContext.SaveChanges();
        }
        //public void SaveInfo(Entities.ProjectTruss projectTruss)
        //{
        //    DAL_DB.ProjectTruss projectTrussesDB = Mapper.Map<DAL_DB.ProjectTruss>(projectTruss);
        //    _trussContext.ProjectTruss.Add(projectTrussesDB);
        //    _trussContext.SaveChanges();
        //}

        public IEnumerable<ProjectTruss> GetAll()
        {
            return _trussContext.ProjectTruss.Select(Mapper.Map<ProjectTruss>);
        }


        public void Add(ProjectTruss projectTruss)
        {
            var projectTrussesDbMap = Mapper.Map<DataAccess.Entities.ProjectTruss>(projectTruss);
            _trussContext.ProjectTruss.Add(projectTrussesDbMap);
            _trussContext.SaveChanges();
            projectTruss.Truss = Mapper.Map<Truss>(_trussContext.Truss.FirstOrDefault(truss => truss.TrussId == projectTruss.TrussId));
            projectTruss.Project = Mapper.Map<Project>(_trussContext.Project.FirstOrDefault(project => project.ProjectId == projectTruss.ProjectId));
        }

        public void Remove(ProjectTruss itemToDelete)
        {
            var trusstoDelete = _trussContext.Truss.First(tr => tr.TrussId == itemToDelete.TrussId);

            _trussContext.ProjectTruss.Remove(_trussContext.ProjectTruss.First(prtr =>
            prtr.TrussId == itemToDelete.TrussId &&
            prtr.ProjectId == itemToDelete.ProjectId &&
            prtr.TrussName == itemToDelete.TrussName
            ));

            if (trusstoDelete.UnitForce != true)
            {
                var bars = from bar in _trussContext.Bar
                           where bar.TrussId == trusstoDelete.TrussId
                           select bar;
                _trussContext.Bar.RemoveRange(bars);
                _trussContext.Truss.Remove(trusstoDelete);
            }

            _trussContext.SaveChanges();
        }

        public void Update(ProjectTruss item)
        {
            var projectTrussesDbFirst = _trussContext.ProjectTruss.First(ptr =>
                ptr.ProjectId == item.ProjectId &&
                ptr.TrussId == item.TrussId && ptr.TrussName == item.TrussName);
            projectTrussesDbFirst.Load = item.Load;
            projectTrussesDbFirst.TrussName = item.TrussName;
            projectTrussesDbFirst.TrussSpacing = item.TrussSpacing;
            _trussContext.SaveChanges();

        }
    }
}
