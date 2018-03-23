using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;

namespace TrussMe.Model.Repositories
{
    public class SteelRepository : ISteelRepository
    {
        private readonly TrussContext _trussContext;
        public SteelRepository(TrussContext trussContext)
        {
            this._trussContext = trussContext;
        }
        public void Add(Steel item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Steel> GetAll()
        {
            return _trussContext.Steel.ToArray().Select(Mapper.Map<Steel>);
        }

        public void Remove(Steel itemToDelete)
        {
            throw new NotImplementedException();
        }

        public void Update(Steel item)
        {
            throw new NotImplementedException();
        }

        public float GetStrength(Steel steel, float thickness)
        {
            return _trussContext.SteelStrength
                .First(ss => ss.SteelId == steel.SteelId && thickness >= ss.MinimumThickness && thickness <= ss.MaximunThickness).YieldStrength;
        }
    }
}
