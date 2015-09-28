using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    internal class MenuItemToolStripMenuItemAdapter : IMenuItem
    {
        private readonly ToolStripMenuItem toolStripMenuItem;

        public MenuItemToolStripMenuItemAdapter(ToolStripMenuItem toolStripMenuItem)
        {
            this.toolStripMenuItem = toolStripMenuItem;
        }

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IMenuItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public int IndexOf(IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IMenuItem IList<IMenuItem>.this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        IMenuItem IMenuItemCollection<IMenuItem>.this[string name]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int IndexOf(string name)
        {
            throw new NotImplementedException();
        }

        public void InsertAfter(string name, IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public void InsertBefore(string name, IMenuItem item)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return toolStripMenuItem.Name; }
            set { toolStripMenuItem.Name = value; }
        }

        public string Text
        {
            get { return toolStripMenuItem.Text; }
            set { toolStripMenuItem.Text = value; }
        }

        public string Tooltip
        {
            get { return ""; }
            set { }
        }

        public string Category
        {
            get { return ""; }
            set { }
        }

        public string Shortcut
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Enabled
        {
            get { return toolStripMenuItem.Enabled; }
            set { toolStripMenuItem.Enabled = value; }
        }

        public bool Visible
        {
            get { return toolStripMenuItem.Visible; }
            set { toolStripMenuItem.Visible = value; }
        }

        public IList<Type> ActiveForViews
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand Command
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}