using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CleanTrack.Models;
using CleanTrack.Mock;
using Microsoft.EntityFrameworkCore;

namespace CleanTrack.Repository;

public class ApplicationDBContext : DbContext
{
    public DbSet<HouseholdChore> HouseholdChores { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
#if DEBUG
        //if (Database.CanConnect()) Database.EnsureDeleted();
#endif
        Database.EnsureCreated();

#if DEBUG
        Generator.HouseholdChores(this);
#endif
    }
}

public class HouseholdChoreRepository : IRepository<HouseholdChore>
{
    private readonly ApplicationDBContext _context;

    public HouseholdChoreRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public IEnumerable<HouseholdChore> GetAll()
    {
        return _context.HouseholdChores.ToList();
    }

    public HouseholdChore GetById(Guid id)
    {
        return _context.HouseholdChores.Find(id);
    }

    public void Add(HouseholdChore entity)
    {
        _context.HouseholdChores.Add(entity);
    }

    public void Update(HouseholdChore entity)
    {
        _context.HouseholdChores.Update(entity);
    }

    public void Delete(HouseholdChore entity)
    {
        _context.HouseholdChores.Remove(entity);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
