using System;
using System.Collections;
using System.Collections.Generic;
using DelftTools.Controls;

namespace DeltaShell.Tests.TestObjects
{
    public class TestMenuItem : IMenuItem
    {
        private readonly IList<IMenuItem> children = new List<IMenuItem>();

        public IList<IMenuItem> Items
        {
            get
            {
                return children;
            }
        }

        public object Tag { get; set; }

        public string Text { get; set; }

        public string Tooltip { get; set; }

        public string Category { get; set; }

        public bool Enabled { get; set; }

        public string Name
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

        #region IMenuItem Members

        public IList<Type> ActiveForViews
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

        #region IEnumerable<IMenuItem> Members

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IMenuItemCollection<IMenuItem> Members

        public IMenuItem this[string name]
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

        public int IndexOf(string name)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public IMenuItem this[int index]
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

        #region ICollection<IMenuItem> Members

        public void Add(IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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