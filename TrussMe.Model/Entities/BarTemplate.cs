using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class BarTemplate : ViewModelBase
    {
        private string _shortName;
        private Section _section;
        private Steel _steel;

        public string ShortName
        {
            get => _shortName;
            set => Set(nameof(ShortName), ref _shortName, value);
        }

        public Section Section
        {
            get => _section;
            set => Set(nameof(Section), ref _section, value);
        }
        
        public Steel Steel
        {
            get => _steel;
            set => Set(nameof(Steel), ref _steel, value);
        }
    }
}
