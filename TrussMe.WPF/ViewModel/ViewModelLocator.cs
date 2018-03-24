using AutoMapper;
using AutoMapper.Configuration;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Interfaces;
using TrussMe.Model.Repositories;

namespace TrussMe.WPF.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            RegisterAutoMapper();
            SimpleIoc.Default.Register<IProjectRepository, ProjectRepository>();
            SimpleIoc.Default.Register<IProjectTrussRepository, ProjectTrussRepository>();
            SimpleIoc.Default.Register<ITrussRepository, TrussRepository>();
            SimpleIoc.Default.Register<ISteelRepository, SteelRepository>();
            SimpleIoc.Default.Register<ISectionRepository, SectionRepository>();
            SimpleIoc.Default.Register<TrussContext>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private static void RegisterAutoMapper()
        {
            var path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            var directoryInfo = new DirectoryInfo(path ?? throw new InvalidOperationException());

            var assemblies = directoryInfo.GetFiles()
                .Where(item => item.Extension.ToLower() == ".dll" && item.Name.StartsWith("TrussMe"))
                .Select(item => Assembly.Load(AssemblyName.GetAssemblyName(item.FullName))).ToList();


            SimpleIoc.Default.Register(
                () =>
                {
                    if (assemblies == null)
                    {
                        throw new ArgumentNullException(nameof(assemblies), @"Collection of assemblies is null.");
                    }

                    var mapperConfigurationExpression = new MapperConfigurationExpression();
                    mapperConfigurationExpression.ConstructServicesUsing(ServiceLocator.Current.GetInstance);

                    mapperConfigurationExpression.AddProfiles(assemblies);

                    var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression);
                    mapperConfiguration.AssertConfigurationIsValid();

                    IMapper mapper = new Mapper(mapperConfiguration, ServiceLocator.Current.GetInstance);

                    return mapper;
                });
        }
    }
}