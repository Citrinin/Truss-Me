using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class Project:ViewModelBase
    {
        private int _projectId;
        private string _code;
        private string _description;

        public int ProjectId
        {
            get => _projectId;
            set => Set(nameof(ProjectId),ref _projectId,value);
        }

        public string Code
        {
            get => _code;
            set => Set(nameof(Code), ref _code, value);
        }

        public string Description
        {
            get => _description;
            set => Set(nameof(Description), ref _description, value);
        }
    }
}
