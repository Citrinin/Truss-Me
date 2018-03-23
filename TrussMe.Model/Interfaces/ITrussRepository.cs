using System.Collections.Generic;
using TrussMe.Model.Entities;

namespace TrussMe.Model.Interfaces
{
    public interface ITrussRepository:IBaseRepository<Truss>
    {
        IEnumerable<TypeOfLoad> GetTypeOfLoads();
        IEnumerable<BarTemplate> GetBarTypes();
    }
}
