using System.Data.Entity;

namespace Application.Ringtoets.Storage
{
    public interface IRingtoetsDbContext
    {
        DbSet<ProjectEntity> ProjectEntities { get; set; }
    }
}
