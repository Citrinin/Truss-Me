using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrussMe.DataAccess.Entities
{
    public class ProjectTruss
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProjectId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrussId { get; set; }

        public float Load { get; set; }

        public int TrussSpacing { get; set; }
        [Key]
        [Index(IsUnique = true)]
        [Column(Order = 2)]
        public string TrussName { get; set; }

        public virtual Project Project { get; set; }

        public virtual Truss Truss { get; set; }
    }
}
