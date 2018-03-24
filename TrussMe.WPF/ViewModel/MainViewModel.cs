using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Wordprocessing;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;
using TrussMe.Model.Services;
using TrussMe.WPF.UserControls;
using Microsoft.Win32;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using System.IO;
using TrussMe.WPF.Converters;
using System.Data.Entity.Infrastructure;
using TrussMe.WPF.Dialogs;

namespace TrussMe.WPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTrussRepository _projectTrussRepository;
        private readonly ITrussRepository _trussRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ISteelRepository _steelRepository;

        private Project _selectedProject;
        private ProjectTruss _selectedProjectTruss;
        private Truss _selectedTruss;
        private BarTemplate _selectedBarTemplate;

        private ObservableCollection<Project> _projectsCollection;
        private ObservableCollection<ProjectTruss> _projectTrussesesCollection;
        private List<BarTemplate> _barTemplateList;
        private ObservableCollection<Bar> _barCollection;
        private ObservableCollection<Section> _sectionCollection;
        private ObservableCollection<Steel> _steelCollection;

        private UserControl _activeUserControl = new StartControl();

        private List<Action> _actions = new List<Action>();
        private bool _shortListOfSections = true;
        private bool _editForces = false;
        private bool _calculated = false;
        private Visibility _resultVisibility = Visibility.Collapsed;
        private IEnumerable<Paragraph> _report;
        private float _trussWeight=0;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            IProjectRepository projectRepository,
            IProjectTrussRepository projectTrussRepository,
            ITrussRepository trussRepository,
            ISectionRepository sectionRepository,
            ISteelRepository steelRepository
        )
        {
            this._projectRepository = projectRepository;
            this._projectTrussRepository = projectTrussRepository;
            this._trussRepository = trussRepository;
            this._sectionRepository = sectionRepository;
            this._steelRepository = steelRepository;

            ProjectsCollection = new ObservableCollection<Project>(this._projectRepository.GetAll());

            #region openProject

            Action openProjectAction = () =>
            {
                ActiveUserControl = new SelectProjectControl();
            };
            this._actions.Add(openProjectAction);
            OpenProjectMenuCommand = new RelayCommand(openProjectAction);

            #endregion

            #region addProject

            this.AddProjectCommand = new RelayCommand(() =>
            {
                AddProjectDialog addNewProj = new AddProjectDialog();
                if (addNewProj.ShowDialog() == true)
                {
                    Project tempProj = addNewProj.NewProject;
                    try
                    {
                        this._projectRepository.Add(tempProj);
                        ProjectsCollection.Add(tempProj);
                    }
                    catch (DbUpdateException e)
                    {
                        MessageBox.Show("Проект с данным кодом уже существует!");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Ошибка при добавлении проекта");
                    }
                }
            });

            #endregion

            #region UpdateProject

            UpdateProjectCommand = new RelayCommand(
                () =>
                {
                    var updateProjectDialog = new UpdateProjectDialog(SelectedProject);
                    if (updateProjectDialog.ShowDialog() == true)
                    {
                        try
                        {
                            this._projectRepository.Update(SelectedProject);
                        }
                        catch (DbUpdateException e)
                        {
                            MessageBox.Show(e.Message);
                        }

                    }
                },
                () => SelectedProject != null);

            #endregion

            #region RemoveProject

            this.RemoveProjectCommand = new RelayCommand(() =>
            {

                if (MessageBox.Show("Вы уверены, что хотите удалить проект?", "Удаление проекта", MessageBoxButton.OKCancel)==MessageBoxResult.OK)
                {
                    try
                    {
                        this._projectRepository.Remove(SelectedProject);
                        ProjectsCollection.Remove(SelectedProject);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Ошибка при удалении проекта");
                    }
                }
            }, () => SelectedProject != null);
            #endregion

            #region openTruss

            Action openTrussMenuAction = () =>
            {
                var x = this._projectRepository.GetProjectTruss(SelectedProject);
                //Получить список ферм!
                ProjectsTrussesCollection = new ObservableCollection<ProjectTruss>(x);

                ActiveUserControl = new SelectProjectTrussControl();

                //MessageBox.Show(SelectedProject.Code);

            };

            this._actions.Add(openTrussMenuAction);


            OpenTrussMenuCommand = new RelayCommand(openTrussMenuAction, () => SelectedProject != null);

            #endregion

            #region addTruss

            AddTrussCommand = new RelayCommand(() =>
            {
                var newTrussDialog = new AddTrussDialog(this._trussRepository.GetTypeOfLoads());
                if (newTrussDialog.ShowDialog() == true)
                {
                    var newProjectTruss = newTrussDialog.NewProjectTruss;
                    var newTruss = newProjectTruss.Truss;
                    this._trussRepository.Add(newTruss);
                    newProjectTruss.Truss = null;
                    newProjectTruss.TrussId = newTruss.TrussId;
                    newProjectTruss.ProjectId = SelectedProject.ProjectId;
                    this._projectTrussRepository.Add(newProjectTruss);
                    ProjectsTrussesCollection.Add(newProjectTruss);
                }
            }, () => SelectedProject != null);

            #endregion

            #region EditTruss

            EditTrussCommand = new RelayCommand(() =>
            {
                var editTussDialog = new AddTrussDialog(SelectedProjectTruss, this._trussRepository.GetTypeOfLoads());
                if (editTussDialog.ShowDialog() == true)
                {
                    this._projectTrussRepository.Update(editTussDialog.NewProjectTruss);
                    editTussDialog.NewProjectTruss.Truss.TrussId = editTussDialog.NewProjectTruss.TrussId;
                    this._trussRepository.Update(editTussDialog.NewProjectTruss.Truss);
                    //SelectedTruss = editTussDialog.NewProjectTruss.Truss;
                }
            }, () => SelectedProjectTruss != null);

            #endregion

            #region RemoveTruss

            RemoveTrussCommand = new RelayCommand(() =>
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить ферму?", "Удаление фермы",
                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        try
                        {
                            this._projectTrussRepository.Remove(SelectedProjectTruss);
                            ProjectsTrussesCollection.Remove(SelectedProjectTruss);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Ошибка при удалении фермы");
                        }

                    }
                },()=>SelectedProjectTruss!=null
                );

             #endregion

            #region DetailTruss

            DetailTrussMenuCommand = new RelayCommand(() =>
            {
                SelectedTruss = SelectedProjectTruss.Truss;
                BarTemplateList = new List<BarTemplate>(this._trussRepository.GetBarTypes());

                SectionCollection = new ObservableCollection<Section>(this._sectionRepository.GetAll().Where(sec => ShortListOfSections? sec.ShortList == ShortListOfSections:true));
                SteelCollection = new ObservableCollection<Steel>(this._steelRepository.GetAll());
                BarCollection = new ObservableCollection<Bar>(SelectedTruss.Bar.OrderBy(bar => bar.BarNumber));

                if (SelectedTruss.UnitForce)
                {
                    BarCollection.AsParallel().ForAll(bar =>
                    {
                        if (bar.SteelId != null)
                        {
                            bar.SteelId = bar.SteelId;
                            bar.Steel = SteelCollection.First(steel => steel.SteelId == bar.SteelId);
                        }
                        if (bar.SectionId != null)
                        {
                            bar.SectionId = bar.SectionId;
                            bar.Section = SectionCollection.First(sec => sec.SectionId == bar.SectionId);
                        }
                    });
                }
                else
                {
                    BarCollection.AsParallel().ForAll(bar =>
                    {
                        if (bar.SteelId != null)
                        {
                            bar.SteelId = bar.SteelId;
                            bar.Steel = SteelCollection.First(steel => steel.SteelId == bar.SteelId);
                        }
                        if (bar.SectionId != null)
                        {
                            bar.SectionId = bar.SectionId;
                            bar.Section = SectionCollection.First(sec => sec.SectionId == bar.SectionId);
                        }
                        bar.ActualForce = bar.Force;
                        bar.ActualMoment = bar.Moment;
                    });
                }


                LoadChangedCommand.Execute(null);

                ActiveUserControl = new TrussDetailControl(SelectedTruss);
            }, () => SelectedProjectTruss != null);

            #endregion

            #region SaveProjectChangesCmd

            SaveChangesCommand = new RelayCommand(() =>
            {
                this._projectTrussRepository.SaveChanges(SelectedProjectTruss);
            }, () => SelectedProjectTruss != null);

            #endregion

            #region LoadChangedCmd

            LoadChangedCommand = new RelayCommand(() =>
            {
                var coeff = SelectedTruss.UnitForce
                    ? SelectedProjectTruss.Load * SelectedProjectTruss.TrussSpacing / 100000
                    : 1;
                foreach (var item in BarCollection)
                {

                    item.ActualForce = item.Force * coeff;
                    item.ActualMoment = item.Moment * coeff;
                }
            }, () => /*ActiveUserControl is TrussDetailControl && */SelectedTruss.UnitForce && SelectedProjectTruss != null);
            #endregion

            #region BarTemplateSelectionChangedCmd

            BarTemplateSelectionChangedCommand = new RelayCommand(() =>
            {
                BarCollection
                    .Where(bar => bar.ElementType == SelectedBarTemplate.ShortName)
                    .AsParallel()
                    .ForAll(bar =>
                    {
                        if (SelectedBarTemplate.Steel != null)
                        {
                            bar.Steel = SelectedBarTemplate.Steel;
                            bar.SteelId = SelectedBarTemplate.Steel.SteelId;
                        }
                        if (SelectedBarTemplate.Section != null)
                        {
                            bar.Section = SelectedBarTemplate.Section;
                            bar.SectionId = SelectedBarTemplate.Section.SectionId;
                        }
                    });
            }, () => SelectedBarTemplate != null);
            #endregion

            #region CalculateTruss

            CalculateTrussCommand = new RelayCommand(() =>
            {
                TrussWeight = 0;
                var f = true;
                foreach (var bar in BarCollection)
                {
                    TrussWeight += bar.Length / 1000F * bar.Section.Mass / 1000F;
                    if (bar.Length == 0 || bar.ActualForce == 0 || bar.Section == null || bar.Steel == null)
                    {
                        f = false;
                        //break;
                        
                    }
                }
                if (f)
                {
                    try
                    {
                        this._report = TrussCalculate.Calculate(SelectedProjectTruss, this._steelRepository);
                        Calculated = true;
                        ResultVisibility = Visibility.Visible;
                    }
                    catch (ArgumentException e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }
                else
                {
                    TrussWeight = 0;
                    MessageBox.Show("Заполните данные о сечениях фермы");
                }
            }, () => SelectedProjectTruss != null);

            #endregion

            #region CreateReport

            CreateReportCommand = new RelayCommand(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word File (*.docx)|*.docx"
                };
                if (saveFileDialog.ShowDialog() == true)
                {

                    var filename = saveFileDialog.FileName.Replace(".docx", "") + ".png";
                    if (ActiveUserControl is TrussDetailControl tdc)
                    {
                        tdc.GetTrussPicture(filename);

                        using (WordprocessingDocument wordprocessingDocument =
                            WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document))
                        {
                            var mainPart = wordprocessingDocument.AddMainDocumentPart();
                            var doc = new Document();
                            mainPart.Document = doc;

                            var body = doc.AppendChild(new Body());
                            ImagePart x = wordprocessingDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);


                            using (FileStream stream = new FileStream(filename, FileMode.Open))
                            {
                                x.FeedData(stream);
                            }

                            AddImageToBody(doc, mainPart.GetIdOfPart(x), tdc.TrussWidthToHeight);
                            CalculateTrussCommand.Execute(null);
                            wordprocessingDocument.MainDocumentPart.Document.Body.Append(this._report);
                            wordprocessingDocument.MainDocumentPart.Document.Save();
                        }
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }


                }
            }, () => this._report != null);

            #endregion

            #region Settings

            SettingsMenuCommand = new RelayCommand(() =>
            {
                this.ActiveUserControl = new SettingsControl();
            });

            #endregion


        }


        public bool ShortListOfSections
        {
            get { return _shortListOfSections; }
            set { Set(nameof(ShortListOfSections), ref _shortListOfSections, value); }
        }
        public bool EditForces
        {
            get { return _editForces; }
            set { Set(nameof(EditForces), ref _editForces, value); }
        }
        public bool Calculated
        {
            get { return this._calculated; }
            set { Set(nameof(Calculated), ref this._calculated, value); }
        }
        public UserControl ActiveUserControl
        {
            get { return this._activeUserControl; }
            set { Set(nameof(ActiveUserControl), ref this._activeUserControl, value); }
        }


        public Project SelectedProject
        {
            get { return this._selectedProject; }
            set { Set(nameof(SelectedProject), ref this._selectedProject, value); }
        }


        public ProjectTruss SelectedProjectTruss
        {
            get { return this._selectedProjectTruss; }
            set { Set(nameof(SelectedProjectTruss), ref this._selectedProjectTruss, value); }
        }

        public Truss SelectedTruss
        {
            get { return this._selectedTruss; }
            set { Set(nameof(SelectedTruss), ref this._selectedTruss, value); }
        }

        public BarTemplate SelectedBarTemplate
        {
            get { return this._selectedBarTemplate; }
            set { Set(nameof(SelectedBarTemplate), ref this._selectedBarTemplate, value); }
        }


        public ObservableCollection<Project> ProjectsCollection
        {
            get { return this._projectsCollection; }
            set { Set(nameof(ProjectsCollection), ref this._projectsCollection, value); }
        }

        public ObservableCollection<ProjectTruss> ProjectsTrussesCollection
        {
            get { return this._projectTrussesesCollection; }
            set { Set(nameof(ProjectsTrussesCollection), ref this._projectTrussesesCollection, value); }
        }
        public List<BarTemplate> BarTemplateList
        {
            get { return this._barTemplateList; }
            set { Set(nameof(BarTemplateList), ref this._barTemplateList, value); }
        }
        public ObservableCollection<Bar> BarCollection
        {
            get { return this._barCollection; }
            set { Set(nameof(BarCollection), ref this._barCollection, value); }
        }
        public ObservableCollection<Section> SectionCollection
        {
            get { return this._sectionCollection; }
            set { Set(nameof(SectionCollection), ref this._sectionCollection, value); }
        }

        public ObservableCollection<Steel> SteelCollection
        {
            get { return this._steelCollection; }
            set { Set(nameof(SteelCollection), ref this._steelCollection, value); }
        }

        public Visibility ResultVisibility
        {
            get { return this._resultVisibility; }
            set { Set(nameof(ResultVisibility), ref this._resultVisibility, value); }
        }
        
        public float TrussWeight
        {
            get { return this._trussWeight; }
            set { Set(nameof(TrussWeight), ref this._trussWeight, value); }
        }

        public RelayCommand OpenProjectMenuCommand { get; set; }
        public RelayCommand OpenTrussMenuCommand { get; set; }
        public RelayCommand DetailTrussMenuCommand { get; set; }
        public RelayCommand LoadChangedCommand { get; set; }
        public RelayCommand BarTemplateSelectionChangedCommand { get; set; }
        public RelayCommand CalculateTrussCommand { get; set; }
        public RelayCommand CreateReportCommand { get; set; }
        public RelayCommand AddProjectCommand { get; set; }
        public RelayCommand RemoveProjectCommand { get; set; }
        public RelayCommand UpdateProjectCommand { get; set; }
        public RelayCommand AddTrussCommand { get; set; }
        public RelayCommand SaveChangesCommand { get; set; }
        public RelayCommand EditTrussCommand { get; set; }
        public RelayCommand RemoveTrussCommand { get; set; }
        public RelayCommand SettingsMenuCommand { get; set; }



        private static void AddImageToBody(Document doc, string relationshipId, double ratio)
        {
            var length = 5940000L;
            var width = (Int64Value)(length / ratio);
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = length, Cy = width },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = length, Cy = width }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            doc.Body.AppendChild(new Paragraph(new Run(element)));
        }
    }
}