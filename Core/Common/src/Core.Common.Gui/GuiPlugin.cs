using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Forms;

namespace Core.Common.Gui
{
    /// <summary>
    /// Template class for gui plugin definitions.
    /// </summary>
    public abstract class GuiPlugin : IDisposable
    {
        /// <summary>
        /// Gets or sets the gui.
        /// </summary>
        public virtual IGui Gui { get; set; }

        /// <summary>
        /// Activates the gui plugin.
        /// </summary>
        public virtual void Activate()
        {

        }

        /// <summary>
        /// Deactivates the gui plugin.
        /// </summary>
        public virtual void Deactivate()
        {

        }

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

        public virtual void Dispose()
        {
            Gui = null;
        }
    }
}