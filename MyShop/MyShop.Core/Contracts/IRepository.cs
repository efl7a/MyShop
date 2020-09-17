using MyShop.Core.Models;
using System.Linq;

namespace MyShop.Core.Contracts
{
    public interface IRepository<T> where T : BaseItem
    {
        IQueryable<T> Collection();
        void Commit();
        void Delete(string Id);
        T Find(string Id);
        void Insert(T p);
        void Update(T item);
    }
}