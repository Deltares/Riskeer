using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.OptionsDialog;

namespace Core.Common.Gui
{
    public abstract class GuiPlugin : IPlugin, IDisposable
    {
        /// <summary>
        /// Reference to the the gui (set by framework)
        /// </summary>
        public virtual IGui Gui { get; set; }

        /// <summary>
        /// Extends ribbon control of the main window.
        /// Override this property to add tabs, groups, buttons or other controls to the ribbon.
        /// </summary>
        public virtual IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<IOptionsControl> OptionsControls
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Provides custom object map layers.
        /// </summary>
        public virtual IMapLayerProvider MapLayerProvider
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  ResourceManger of plugin. Default Properties.Resources.
        ///  </summary>
        public virtual ResourceManager Resources { get; set; }

        /// <summary>
        /// Returns all property information objects supported by the plugin
        /// </summary>
        public virtual IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            return Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Provides views info objects for creating views.
        /// </summary>
        public virtual IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield break;
        }

        public virtual void OnViewAdded(IView view) {}

        public virtual void OnViewRemoved(IView view) {}

        public virtual void OnActiveViewChanged(IView view) {}

        /// <summary>
        /// Returns a context menu which is used for this object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ContextMenuStrip GetContextMenu(object sender, object data)
        {
            return null;
        }

        public virtual IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield break;
        }

        /// <summary>
        /// TODO: is it not part of IView?
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public virtual void OnDragDrop(object source, object target) {}

        /// <summary>
        /// TODO: is it not part of IView?
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public virtual bool CanDrop(object source, object target)
        {
            return false;
        }

        /// <summary>
        /// Returns false if plugin does not allow to paste <paramref name="item"/> into <paramref name="container"/>.
        /// 
        /// Return true in default implementation.
        /// </summary>
        public virtual bool CanPaste(IProjectItem item, IProjectItem container)
        {
            return true;
        }

        /// <summary>
        /// Returns false if plugin does not allow to copy <paramref name="item"/> for copy/paste action.
        /// 
        /// Return true in default implementation.
        /// </summary>
        public virtual bool CanCopy(IProjectItem item)
        {
            return true;
        }

        /// <summary>
        /// Returns false if plugin does not allow to cut <paramref name="item"/> for copy/paste action.
        /// 
        /// Return true in default implementation.
        /// </summary>
        public virtual bool CanCut(IProjectItem item)
        {
            return true;
        }

        /// <summary>
        /// Returns false when data item can not be deleted by the user. 
        /// </summary>
        public virtual bool CanDelete(IProjectItem item)
        {
            return true;
        }

        public virtual void Dispose()
        {
            Gui = null;
        }

        /// <summary>
        ///  Activates the plugin.
        ///  </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        ///  Deactivates the plugin.
        ///  </summary>
        public virtual void Deactivate()
        {

        }
    }
}