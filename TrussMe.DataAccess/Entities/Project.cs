using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrussMe.DataAccess.Entities
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Index(IsUnique = true)]
        [StringLength(250)]
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ProjectTruss> ProjectTruss { get; set; }
    }
}
