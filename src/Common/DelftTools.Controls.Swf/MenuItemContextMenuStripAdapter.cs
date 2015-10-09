using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using log4net;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// Wraps System.Windows.Forms.ContextMenuStrip so it behaves like IMenuItem.
    /// </summary>
    public class MenuItemContextMenuStripAdapter : IMenuItem, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MenuItemContextMenuStripAdapter));

        public MenuItemContextMenuStripAdapter(ContextMenuStrip contextMenuStrip)
        {
            ContextMenuStrip = contextMenuStrip;
        }

        /// <summary>
        /// The contextmenustrip wrapped by this adapter.
        /// </summary>
        public ContextMenuStrip ContextMenuStrip { get; private set; }

        public void Dispose()
        {
            if (ContextMenuStrip != null)
            {
                ContextMenuStrip.Dispose();
            }
        }

        #region IEnumerable<IMenuItem> Members

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ContextMenuStrip.Items.GetEnumerator();
        }

        #endregion

        #region IMenuItem Members

        public string Name
        {
            get
            {
                return ContextMenuStrip.Name;
            }
            set
            {
                ContextMenuStrip.Name = value;
            }
        }

        public string Text
        {
            get
            {
                return ContextMenuStrip.Text;
            }
            set
            {
                ContextMenuStrip.Text = value;
            }
        }

        public string Tooltip
        {
            get
            {
                return "";
            }
            set {}
        }

        public string Category
        {
            get
            {
                return "";
            }
            set {}
        }

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public string Shortcut
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public IList<Type> ActiveForViews
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public ICommand Command
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IMenuItemCollection<IMenuItem> Members

        public IMenuItem this[string name]
        {
            get
            {
                //ToolStripItem 
                //ToolStripMenuItem
                var menuItem = ContextMenuStrip.Items[name];
                if (menuItem is ToolStripMenuItem)
                {
                    return new MenuItemToolStripMenuItemAdapter((ToolStripMenuItem) menuItem);
                }
                return null;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public int IndexOf(string name)
        {
            var menuItem = ContextMenuStrip.Items[name];
            if (null != menuItem)
            {
                return ContextMenuStrip.Items.IndexOf(menuItem);
            }
            return -1;
        }

        public void InsertAfter(string name, IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void InsertBefore(string name, IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IList<IMenuItem> Members

        public int IndexOf(IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void Insert(int index, IMenuItem item)
        {
            if (item is MenuItemContextMenuStripAdapter)
            {
                var items = ((MenuItemContextMenuStripAdapter) item).ContextMenuStrip.Items;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i] is ClonableToolStripMenuItem)
                    {
                        ContextMenuStrip.Items.Insert(index, ((ClonableToolStripMenuItem) items[i]).Clone());
                    }
                    else if (items[i] is ToolStripSeparator)
                    {
                        ContextMenuStrip.Items.Insert(index, new ToolStripSeparator()
                        {
                            Available = items[i].Available
                        });
                    }
                    else
                    {
                        log.Error(string.Format("Menuitem {0} not clonable.", items[i].Text));
                    }
                }
            }
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public IMenuItem this[int index]
        {
            get
            {
                var menuItem = ContextMenuStrip.Items[index];
                if (menuItem is ToolStripMenuItem)
                {
                    return new MenuItemToolStripMenuItemAdapter((ToolStripMenuItem) menuItem);
                }
                return null;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<IMenuItem> Members

        public void Add(IMenuItem item)
        {
            if (item is MenuItemContextMenuStripAdapter)
            {
                var menuItemContextMenuStripAdapter = (MenuItemContextMenuStripAdapter) item;

                for (int i = 0; i < menuItemContextMenuStripAdapter.ContextMenuStrip.Items.Count; i++)
                {
                    ToolStripItem toolStripItem = menuItemContextMenuStripAdapter.ContextMenuStrip.Items[i];
                    if (toolStripItem is ClonableToolStripMenuItem)
                    {
                        var clonableToolStripMenuItem =
                            (ClonableToolStripMenuItem) menuItemContextMenuStripAdapter.ContextMenuStrip.Items[i];
                        ContextMenuStrip.Items.Add(clonableToolStripMenuItem.Clone());
                    }
                    else if (toolStripItem is ToolStripSeparator)
                    {
                        ContextMenuStrip.Items.Add(new ToolStripSeparator()
                        {
                            Available = toolStripItem.Available
                        });
                    }
                    else
                    {
                        log.Error(string.Format("Menuitem {0} not clonable.", menuItemContextMenuStripAdapter.ContextMenuStrip.Items[i].Text));
                    }
                }
            }
            else
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public void Clear()
        {
            //throw new NotImplementedException("The method or operation is not implemented.");
            ContextMenuStrip.Items.Clear();
        }

        public bool Contains(IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void CopyTo(IMenuItem[] array, int arrayIndex)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public int Count
        {
            get
            {
                return ContextMenuStrip.Items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public bool Remove(IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion
    }
}