using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class TypeOfLoad:ViewModelBase
    {
        private int _loadId;
        private string _loadType;

        public int LoadId
        {
            get => _loadId;
            set => Set(nameof(LoadId), ref _loadId, value);
        }

        public string LoadType
        {
            get => _loadType;
            set => Set(nameof(LoadType), ref _loadType, value);
        }
    }
}
