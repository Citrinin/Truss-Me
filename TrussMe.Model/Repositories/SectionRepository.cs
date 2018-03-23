using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;

namespace TrussMe.Model.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly TrussContext _trussContext;

        public SectionRepository()
        {
            _trussContext = new TrussContext();
        }

        public void Add(Section item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Section> GetAll()
        {
            return _trussContext.Section.ToArray().Select(Mapper.Map<Section>);
        }

        public void Remove(Section itemToDelete)
        {
            throw new NotImplementedException();
        }

        public void Update(Section item)
        {
            throw new NotImplementedException();
        }
    }
}
