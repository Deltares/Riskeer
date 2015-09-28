using DelftTools.Shell.Core.Dao;

namespace DeltaShell.Plugins.CommonTools
{
    public class CommonToolsDataAccessListener : DataAccessListenerBase
    {
        public override object Clone()
        {
            return new CommonToolsDataAccessListener {ProjectRepository = ProjectRepository};
        }
    }
}