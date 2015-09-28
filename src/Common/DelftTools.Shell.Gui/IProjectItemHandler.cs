using DelftTools.Shell.Core;

namespace DelftTools.Shell.Gui
{
    interface IProjectItemHandler
    {
        /// <summary>
        /// Returns false if plugin does not allow to paste <paramref name="item"/> into <paramref name="container"/>.
        /// </summary>
        bool CanPaste(IProjectItem item, IProjectItem container);

        /// <summary>
        /// Returns false if plugin does not allow to copy <paramref name="item"/> for copy/paste action.
        /// </summary>
        bool CanCopy(IProjectItem item);

        /// <summary>
        /// Returns false if plugin does not allow to cut <paramref name="item"/> for copy/paste action.
        /// </summary>
        bool CanCut(IProjectItem item);

        /// <summary>
        /// Returns false when data item can not be deleted by the user. 
        /// </summary>
        bool CanDelete(IProjectItem item);
    }

    interface IDragDropHandler
    {
        /// <summary>
        /// TODO: is it not part of IView?
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        bool CanDrop(object source, object target);

        /// <summary>
        /// TODO: is it not part of IView?
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void OnDragDrop(object source, object target);
    }
}
