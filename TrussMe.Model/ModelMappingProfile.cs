using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using ProjectDA = TrussMe.DataAccess.Entities.Project;
using ProjectTrussDA = TrussMe.DataAccess.Entities.ProjectTruss;
using TrussDA = TrussMe.DataAccess.Entities.Truss;
using BarDA = TrussMe.DataAccess.Entities.Bar;
using SectionDA = TrussMe.DataAccess.Entities.Section;
using SteelDA = TrussMe.DataAccess.Entities.Steel;
using TrussElementDA = TrussMe.DataAccess.Entities.TrussElement;
using TypeOfLoadDA = TrussMe.DataAccess.Entities.TypeOfLoad;
using SteelStrengthDA = TrussMe.DataAccess.Entities.SteelStrength;
using TrussMe.Model.Entities;

namespace TrussMe.Model
{
    public static class ModelMappingProfile
    {
        public static void MapperInitializer(TrussContext trussContext)
        {

                Mapper.Initialize(
                    cfg =>
                    {
                        cfg.CreateMap<ProjectDA, Project>();
                        cfg.CreateMap<Project, ProjectDA>()
                            .ForMember(pda => pda.ProjectTruss, opt => opt.Ignore());
                        cfg.CreateMap<TrussDA, Truss>()
                            .ForMember("LoadType", opt => opt.MapFrom(c => trussContext.TypeOfLoad.First(lt => lt.LoadId == c.LoadId).LoadType));
                        cfg.CreateMap<Truss, TrussDA>()
                            .ForMember("ProjectTruss", opt => opt.Ignore())
                            .ForMember("TypeOfLoad", opt => opt.Ignore());
                        cfg.CreateMap<ProjectTrussDA, ProjectTruss>();
                        cfg.CreateMap<ProjectTruss, ProjectTrussDA>();
                        cfg.CreateMap<BarDA, Bar>()
                            .ForMember("ElementType", opt => opt.MapFrom(c => trussContext.TrussElement.First(trEl => trEl.ElementId == c.ElementId).ShortName));
                        cfg.CreateMap<Bar, BarDA>();
                        cfg.CreateMap<SectionDA, Section>();
                        cfg.CreateMap<Section, SectionDA>();
                        cfg.CreateMap<SteelStrengthDA, SteelStrength>();
                        cfg.CreateMap<SteelStrength, SteelStrengthDA>();
                        cfg.CreateMap<SteelDA, Steel>();
                        cfg.CreateMap<Steel, SteelDA>();
                        cfg.CreateMap<TrussElementDA, BarTemplate>();
                        cfg.CreateMap<TypeOfLoad, TypeOfLoadDA>();
                        cfg.CreateMap<TypeOfLoadDA, TypeOfLoad>();
                    }
            );



                //.ForMember(pt=>pt.Truss,opt => opt.ResolveUsing((projectTrussesDataAccess, projectTrusses, i, context) => context.Mapper.Map<Truss>(projectTrussesDataAccess.Truss)))
                //.ForMember(pt=>pt.Project, opt=> opt.MapFrom(ptDA=>ptDA.Project));





                //   // .ForMember("LoadType", opt => opt.MapFrom(c => trussContext.TypeOfLoad.First(lt => lt.LoadId == c.LoadId).LoadType));
                //this.CreateMap<Truss, TrussDA>()
                //    .ForMember(truss => truss.ProjectTruss, opt => opt.Ignore())
                //    .ForMember(truss => truss.TypeOfLoad, opt => opt.Ignore());


                //this.CreateMap<BarDA, Bar>()
                //    .ForMember("ElementType", opt => opt.MapFrom(c => trussContext.TrussElement.First(trEl => trEl.ElementId == c.ElementId).ShortName));
                //this.CreateMap<Bar, BarDA>();
                //this.CreateMap<SectionDA, Section>();
                //this.CreateMap<Section, SectionDA>();
                //this.CreateMap<SteelDA, Steel>();
                //this.CreateMap<Steel, SteelDA>();
                //this.CreateMap<TrussElementDA, BarTemplate>();
                //this.CreateMap<ProjectTrussesDA, ProjectTruss>()
                //    ////???
                //    .ForMember("Truss", opt => opt.MapFrom(c => Mapper.Map<Truss>(c.Truss)));
                //this.CreateMap<ProjectTruss, ProjectTrussesDA>();
                //this.CreateMap<TypeOfLoad, TypeOfLoadDA>();
                //this.CreateMap<TypeOfLoadDA, TypeOfLoad>();

        }

    }
}
