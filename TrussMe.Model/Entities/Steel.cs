using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class Steel : ViewModelBase
    {
        private int _steelId;
        private string _grade;

        public int SteelId
        {
            get => _steelId;
            set => Set(nameof(SteelId), ref _steelId, value);
        }

        public string Grade
        {
            get => _grade;
            set => Set(nameof(Grade), ref _grade, value);
        }

        public IEnumerable<SteelStrength> SteelStrength { get; set; }
    }
}
