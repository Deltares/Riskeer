using DelftTools.Utils;
using DelftTools.Utils.Data;

namespace DelftTools.Shell.Core
{
    ///<summary>
    /// Folder Item
    /// TODO: remove IDeepCloneable, not all project items must be cloneable, also use ICloneable
    /// TODO: rename to IEntity and move to DelftTools.Utils, it has nothing to do with Folder
    ///</summary>
    public interface IProjectItem : IUnique<long>, INameable, IDeepCloneable
    {
    }
}