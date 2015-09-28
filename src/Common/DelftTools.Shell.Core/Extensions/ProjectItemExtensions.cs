//namespace DelftTools.Shell.Core.Extensions
//{
//    ///<summary>
//    /// Extension (helper) methods defined on the IProjectItem and IItemContainer interfaces.
//    /// The use of extension methods keeps concrete classes clean and also the interface itself doesn't have to 
//    /// be extended so we dont break subclasses..
//    ///</summary>
//    public static class ProjectItemExtensions
//    {
//        public static IProjectItem Owner(this IProjectItem child)
//        {
//            if (child is IProjectItemOwned)
//                return ((IProjectItemOwned)child).Owner;
//
//            return null;
//        }
//    }
//}