using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class SteelStrength : ViewModelBase
    {
        private int _steelId;
        private int _minimumThickness;
        private int _maximunThickness;
        private float _yieldStrength;
        private float _ultimateStrength;

        public int SteelId
        {
            get => _steelId;
            set => Set(nameof(SteelId), ref _steelId, value);
        }

        public int MinimumThickness
        {
            get => _minimumThickness;
            set => Set(nameof(MinimumThickness), ref _minimumThickness, value);
        }

        public int MaximunThickness
        {
            get => _maximunThickness;
            set => Set(nameof(MaximunThickness), ref _maximunThickness, value);
        }

        public float YieldStrength
        {
            get => _yieldStrength;
            set => Set(nameof(YieldStrength), ref _yieldStrength, value);
        }

        public float UltimateStrength
        {
            get => _ultimateStrength;
            set => Set(nameof(UltimateStrength), ref _ultimateStrength, value);
        }
    }
}
