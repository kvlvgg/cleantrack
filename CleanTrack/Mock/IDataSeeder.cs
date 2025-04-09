#if DEBUG
using CleanTrack.Repository;

namespace CleanTrack.Mock
{
    public interface IDataSeeder
    {
        void Seed(ApplicationDBContext context);
    }
}
#endif