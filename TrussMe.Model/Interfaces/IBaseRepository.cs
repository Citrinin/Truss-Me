using System.Collections.Generic;

namespace TrussMe.Model.Interfaces
{
    public interface IBaseRepository<T> where T:class
    {
        IEnumerable<T> GetAll();
        void Add(T item);
        void Remove(T itemToDelete);
        void Update(T item);
    }
}
