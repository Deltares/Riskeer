using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage
{
    public interface IStoreProject
    {
        void SaveProject(Project project);

        Project LoadProject(long projectId);
    }
}