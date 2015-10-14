using DelftTools.Shell.Core.Dao;

namespace DeltaShell.Plugins.CommonTools
{
    public class CommonToolsDataAccessListener : DataAccessListenerBase
    {
        /// <summary>
        /// Creates a new CommonToolsDataAccessListener that is a copy of the current instance.
        /// Derived from <see cref="DataAccessListenerBase.Clone">DataAccessListenerBase.Clone()</see>
        /// </summary>
        /// <returns>A new CommonToolsDataAccessListener that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new CommonToolsDataAccessListener
            {
                ProjectRepository = ProjectRepository
            };
        }
    }
}