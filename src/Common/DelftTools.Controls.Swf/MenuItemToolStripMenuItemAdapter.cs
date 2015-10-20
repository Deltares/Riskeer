using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    internal class MenuItemToolStripMenuItemAdapter : IMenuItem
    {
        public MenuItemToolStripMenuItemAdapter(ToolStripMenuItem toolStripMenuItem)
        {

        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}