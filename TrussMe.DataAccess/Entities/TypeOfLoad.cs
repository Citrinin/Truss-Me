using System.ComponentModel.DataAnnotations;

namespace TrussMe.DataAccess.Entities
{
    public class TypeOfLoad
    {
        [Key]
        public int LoadId { get; set; }
        public string LoadType { get; set; }
    }
}

