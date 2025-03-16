using CleanTimer.Models;

using Microsoft.EntityFrameworkCore;

namespace CleanTimer.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Delete(T entity);
        void Save();
    }
}
