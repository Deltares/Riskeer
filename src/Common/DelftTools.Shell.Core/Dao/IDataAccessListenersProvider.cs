using System.Collections.Generic;

namespace DelftTools.Shell.Core.Dao
{
    public interface IDataAccessListenersProvider
    {
        IEnumerable<IDataAccessListener> CreateDataAccessListeners();
    }
}