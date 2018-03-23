using TrussMe.Model.Entities;

namespace TrussMe.Model.Interfaces
{
    public interface ISteelRepository : IBaseRepository<Steel>
    {
        float GetStrength(Steel steel, float thickness);
    }
}
