using CleanTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanTrack.Repository;

public class ApplicationDBContext : DbContext
{
    public DbSet<Chore> Chores { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}

public class ChoreRepository : IRepository<Chore>
{
    private readonly ApplicationDBContext _context;

    public ChoreRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public IEnumerable<Chore> GetAll()
    {
        return _context.Chores.ToList();
    }

    public Chore GetById(Guid id)
    {
        return _context.Chores.Find(id);
    }

    public void Add(Chore entity)
    {
        _context.Chores.Add(entity);
    }

    public void Update(Chore entity)
    {
        _context.Chores.Update(entity);
    }

    public void Delete(Chore entity)
    {
        _context.Chores.Remove(entity);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
