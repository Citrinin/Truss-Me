using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class ProjectTruss : ViewModelBase
    {
        private int _projectId;
        private int _trussId;
        private float _load;
        private int _trussSpacing;
        private string _trussName;
        private Truss _truss;
        private Project _project;

        public int ProjectId
        {
            get => _projectId;
            set => Set(nameof(ProjectId), ref _projectId, value);
        }

        public int TrussId
        {
            get => _trussId;
            set => Set(nameof(TrussId), ref _trussId, value);
        }

        public float Load
        {
            get => _load;
            set => Set(nameof(Load), ref _load, value);
        }

        public int TrussSpacing
        {
            get => _trussSpacing;
            set => Set(nameof(TrussSpacing), ref _trussSpacing, value);
        }

        public string TrussName
        {
            get => _trussName;
            set => Set(nameof(TrussName), ref _trussName, value);
        }

        public Truss Truss
        {
            get => _truss;
            set => Set(nameof(Truss), ref _truss, value);
        }

        public Project Project
        {
            get => _project;
            set => Set(nameof(Project), ref _project, value);
        }
    }
}
