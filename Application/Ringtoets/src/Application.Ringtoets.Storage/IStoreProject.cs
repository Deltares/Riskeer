using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage
{
    public interface IStoreProject
    {
        int SaveProject(Project project);

        Project LoadProject(long projectId);
    }
}