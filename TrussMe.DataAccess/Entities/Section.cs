namespace TrussMe.DataAccess.Entities
{
    public class Section
    {
        public int SectionId { get; set; }

        public string SectionName { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public float Thickness { get; set; }

        public float Area { get; set; }

        public float Mass { get; set; }

        public float MomentOfInertiaX { get; set; }

        public float MomentOfInertiaY { get; set; }

        public float SectionModulusX { get; set; }

        public float SectionModulusY { get; set; }

        public float RadiusOfGyrationX { get; set; }

        public float RadiusOfGyrationY { get; set; }

        public bool ShortList { get; set; }

        public bool Square { get; set; }

    }
}
