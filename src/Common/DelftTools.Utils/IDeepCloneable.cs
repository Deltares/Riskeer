using System;

namespace DelftTools.Utils
{
    ///<summary>
    /// See "Deep copy" section on http://en.wikipedia.org/wiki/Object_copy
    /// TODO: agree to use ICloneable as a deep clone (how deep? :)) and remove it, we don't use shallow clone so we need only 1 type of clone
    /// If more types of clone are needed - then it is not clone, but context-based: Import, TransformToOtherObject, etc. Then solve it at the place where this action is implemented
    ///</summary>
    public interface IDeepCloneable
    {
        ///<summary>
        /// Gets a deep copy of the object and its relations 
        ///</summary>
        ///<returns>The object graph</returns>
        object DeepClone();    
    }

    public interface IManualCloneable : ICloneable
    {
    }
}