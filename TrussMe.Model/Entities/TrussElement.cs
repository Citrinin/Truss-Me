using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class TrussElement:ViewModelBase
    {
        private int _elementId;
        private string _fullName;
        private string _shortName;

        public int ElementId
        {
            get => _elementId;
            set => Set(nameof(ElementId), ref _elementId, value);
        }
        
        public string FullName
        {
            get => _fullName;
            set => Set(nameof(FullName), ref _fullName, value);
        }
        
        public string ShortName
        {
            get => _shortName;
            set => Set(nameof(ShortName), ref _shortName, value);
        }
    }
}
