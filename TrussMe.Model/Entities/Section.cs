using GalaSoft.MvvmLight;

namespace TrussMe.Model.Entities
{
    public class Section : ViewModelBase
    {
        private int _sectionId;
        private string _sectionName;
        private int _height;
        private int _width;
        private float _thickness;
        private float _area;
        private float _mass;
        private float _momentOfInertiaX;
        private float _momentOfInertiaY;
        private float _sectionModulusX;
        private float _sectionModulusY;
        private float _radiusOfGyrationX;
        private float _radiusOfGyrationY;
        private bool _shortList;
        private bool _square;

        public int SectionId
        {
            get => _sectionId;
            set => Set(nameof(SectionId), ref _sectionId, value);
        }

        public string SectionName
        {
            get => _sectionName;
            set => Set(nameof(SectionName), ref _sectionName, value);
        }

        public int Height
        {
            get => _height;
            set => Set(nameof(Height), ref _height, value);
        }

        public int Width
        {
            get => _width;
            set => Set(nameof(Width), ref _width, value);
        }

        public float Thickness
        {
            get => _thickness;
            set => Set(nameof(Thickness), ref _thickness, value);
        }

        public float Area
        {
            get => _area;
            set => Set(nameof(Area), ref _area, value);
        }

        public float Mass
        {
            get => _mass;
            set => Set(nameof(Mass), ref _mass, value);
        }

        public float MomentOfInertiaX
        {
            get => _momentOfInertiaX;
            set => Set(nameof(MomentOfInertiaX), ref _momentOfInertiaX, value);
        }

        public float MomentOfInertiaY
        {
            get => _momentOfInertiaY;
            set => Set(nameof(MomentOfInertiaY), ref _momentOfInertiaY, value);
        }

        public float SectionModulusX
        {
            get => _sectionModulusX;
            set => Set(nameof(SectionModulusX), ref _sectionModulusX, value);
        }

        public float SectionModulusY
        {
            get => _sectionModulusY;
            set => Set(nameof(SectionModulusY), ref _sectionModulusY, value);
        }
        
        public float RadiusOfGyrationX
        {
            get => _radiusOfGyrationX;
            set => Set(nameof(RadiusOfGyrationX), ref _radiusOfGyrationX, value);
        }
        
        public float RadiusOfGyrationY
        {
            get => _radiusOfGyrationY;
            set => Set(nameof(RadiusOfGyrationY), ref _radiusOfGyrationY, value);
        }
        
        public bool ShortList
        {
            get => _shortList;
            set => Set(nameof(ShortList), ref _shortList, value);
        }
        
        public bool Square
        {
            get => _square;
            set => Set(nameof(Square), ref _square, value);
        }
    }
}
