using GalaSoft.MvvmLight;


namespace TrussMe.Model.Entities
{
    public class Bar : ViewModelBase
    {
        private int _barId;

        public int BarId
        {
            get => _barId;
            set => Set(nameof(BarId), ref _barId, value);
        }

        private float _length;

        public float Length
        {
            get => _length;
            set => Set(nameof(Length), ref _length, value);
        }

        private float _force;

        public float Force
        {
            get => _force;
            set => Set(nameof(Force), ref _force, value);
        }

        private float _moment;

        public float Moment
        {
            get => _moment;
            set => Set(nameof(Moment), ref _moment, value);
        }

        private int _trussId;

        public int TrussId
        {
            get => _trussId;
            set => Set(nameof(TrussId), ref _trussId, value);
        }

        private int? _sectionId;

        public int? SectionId
        {
            get => _sectionId;
            set => Set(nameof(SectionId), ref _sectionId, value);
        }

        private int? _steelId;

        public int? SteelId
        {
            get => _steelId;
            set => Set(nameof(SteelId), ref _steelId, value);
        }

        private int _elementId;

        public int ElementId
        {
            get => _elementId;
            set => Set(nameof(ElementId), ref _elementId, value);
        }

        private string _elementType;

        public string ElementType
        {
            get => _elementType;
            set => Set(nameof(ElementType), ref _elementType, value);
        }


        private int _barNumber;

        public int BarNumber
        {
            get => _barNumber;
            set => Set(nameof(BarNumber), ref _barNumber, value);
        }
        private float _actualForce;

        public float ActualForce
        {
            get => _actualForce;
            set => Set(nameof(ActualForce),ref _actualForce, value);
        }

        private float _actualMoment;

        public float ActualMoment
        {
            get => _actualMoment;
            set => Set(nameof(ActualMoment),ref _actualMoment, value);
        }

        private Section _section;

        public Section Section
        {
            get => _section;
            set => Set(nameof(Section), ref _section, value);
        }
        private Steel _steel;

        public Steel Steel
        {
            get => _steel;
            set => Set(nameof(Steel), ref _steel, value);
        }

        private float _calcResult;
        public float CalcResult
        {
            get => _calcResult;
            set => Set(nameof(CalcResult), ref _calcResult, value);
        }

        private string _calcType;
        public string CalcType
        {
            get => _calcType;
            set => Set(nameof(CalcType), ref _calcType, value);
        }
    }
}
