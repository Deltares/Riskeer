using System;
using DelftTools.Utils.Collections;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    public class TreeClient
    {
        public event EventHandler<EventArgs> RootChanged;
        private ClassImplementingIEventedList root;

        public TreeClient()
        {
            Root = new ClassImplementingIEventedList();
        }

        public ClassImplementingIEventedList Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
                ((INotifyCollectionChange) root).CollectionChanged += root_CollectionChanged;
            }
        }

        private void root_CollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (RootChanged != null)
            {
                RootChanged(this, new EventArgs());
            }
        }
    }
}