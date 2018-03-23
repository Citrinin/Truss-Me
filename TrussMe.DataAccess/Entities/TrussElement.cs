using System.ComponentModel.DataAnnotations;

namespace TrussMe.DataAccess.Entities
{
    public class TrussElement
    {
        [Key]
        public int ElementId { get; set; }
        
        public string FullName { get; set; }


        public string ShortName { get; set; }
        
    }
}

