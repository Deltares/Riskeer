using System;
using System.Collections.Generic;
using DelftTools.Controls;
using DelftTools.Shell.Gui;

namespace DeltaShell.Tests.TestObjects
{
    class TestMenuItem: IMenuItem
    {
        private readonly IList<IMenuItem> children = new List<IMenuItem>();
        private string text;
        private bool enabled;
        private object tag;

        public IList<IMenuItem> Items 
        { 
            get { return children; } 
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Tooltip { get; set; }

        public string Category { get; set; }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


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


        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public event EventHandler Click;

        public IMenuItem AppendSub(bool newGroup, IGuiCommand command, EventHandler eventHandler)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public bool Remove(IMenuItem item)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<IMenuItem> Members

        public IEnumerator<IMenuItem> GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

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

    }
}