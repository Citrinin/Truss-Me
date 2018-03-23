using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrussMe.DataAccess.Entities
{
    public class SteelStrength
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SteelId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MinimumThickness { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaximunThickness { get; set; }

        public float YieldStrength { get; set; }

        public float UltimateStrength { get; set; }

        public virtual Steel Steel { get; set; }
    }
}
