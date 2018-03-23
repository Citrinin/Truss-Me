using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class Truss : ViewModelBase
    {
        private int _trussId;
        private int _span;
        private float _slope;
        private int _supportDepth;
        private int _panelAmount;
        private int _loadId;
        private string _loadType;
        private bool _unitForce;

        public int TrussId
        {
            get => _trussId;
            set => Set(nameof(TrussId), ref _trussId, value);
        }

        public int Span
        {
            get => _span;
            set => Set(nameof(Span), ref _span, value);
        }

        public float Slope
        {
            get => _slope;
            set => Set(nameof(Slope), ref _slope, value);
        }

        public int SupportDepth
        {
            get => _supportDepth;
            set => Set(nameof(SupportDepth), ref _supportDepth, value);
        }

        public int PanelAmount
        {
            get => _panelAmount;
            set => Set(nameof(PanelAmount), ref _panelAmount, value);
        }

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


        public bool UnitForce
        {
            get => _unitForce;
            set { Set(nameof(UnitForce), ref _unitForce, value); RaisePropertyChanged(nameof(NotUnitForce)); }
        }

        public bool NotUnitForce => !_unitForce;

        public IEnumerable<Bar> Bar { get; set; }

    }
}
