using System.Collections.Generic;

namespace TrussMe.DataAccess.Entities
{
    public class Steel
    {
        public int SteelId { get; set; }

        public string Grade { get; set; }

        public virtual ICollection<SteelStrength> SteelStrength { get; set; }
    }
}

