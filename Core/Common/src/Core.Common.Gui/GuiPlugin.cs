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
        /// Ribbon command handler (adding tabs, groups, buttons, etc.) which can be provided by the gui plugin.
        /// </summary>
        public virtual IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Property info objects which can be provided by the gui plugin.
        /// </summary>
        public virtual IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            return Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// View information objects which can be provided by the gui plugin.
        /// </summary>
        public virtual IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield break;
        }

        /// <summary>
        /// Context menu which can be provided by the gui plugin.
        /// </summary>
        public virtual ContextMenuStrip GetContextMenu(object sender, object data)
        {
            return null;
        }

        /// <summary>
        /// Node presenters which can be provided by the gui plugin.
        /// </summary>
        public virtual IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield break;
        }

        public virtual void Dispose()
        {
            Gui = null;
        }

        public virtual void OnViewRemoved(IView view) { }

        public virtual void OnActiveViewChanged(IView view) { }
    }
}