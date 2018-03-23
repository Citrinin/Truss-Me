using System.Collections.Generic;

namespace TrussMe.DataAccess.Entities
{
    public class Truss
    {
        public int TrussId { get; set; }
        public int Span { get; set; }
        public float Slope { get; set; }
        public int SupportDepth { get; set; }
        public int PanelAmount { get; set; }
        public int LoadId { get; set; }
        public bool UnitForce { get; set; }
        public virtual ICollection<Bar> Bar { get; set; }
        public virtual ICollection<ProjectTruss> ProjectTruss { get; set; }
        public virtual TypeOfLoad TypeOfLoad { get; set; }

    }
}
