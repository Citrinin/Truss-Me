namespace TrussMe.DataAccess.Entities
{
    public class Bar
    {
        public int BarId { get; set; }

        public float Length { get; set; }

        public float Force { get; set; }

        public float Moment { get; set; }

        public int TrussId { get; set; }

        public int? SectionId { get; set; }

        public int? SteelId { get; set; }

        public int ElementId { get; set; }

        public int BarNumber { get; set; }

        public virtual Truss Truss { get; set; }

        public virtual Section Section { get; set; }

        public virtual Steel Steel { get; set; }

        public virtual TrussElement TrussElement { get; set; }
    }
}
