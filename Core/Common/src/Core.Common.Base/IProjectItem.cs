using Core.Common.Utils;

namespace Core.Common.Base
{
    ///<summary>
    /// Folder Item
    /// TODO: remove IDeepCloneable, not all project items must be cloneable, also use ICloneable
    /// TODO: rename to IEntity and move to Core.Common.Utils, it has nothing to do with Folder
    ///</summary>
    public interface IProjectItem : IDeepCloneable {}
}