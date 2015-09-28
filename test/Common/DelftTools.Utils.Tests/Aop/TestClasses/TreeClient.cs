using System;
using DelftTools.Utils.Collections;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    public class TreeClient
    {
        private ClassImplementingIEventedList root;

        public TreeClient()
        {
            Root = new ClassImplementingIEventedList();
        }

        public Utils.Tests.Aop.TestClasses.ClassImplementingIEventedList Root
        {
            get { return root; }
            set
            {
                root = value;
                ((INotifyCollectionChange)root).CollectionChanged += root_CollectionChanged;
            }
        }

        void root_CollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (RootChanged != null)
                RootChanged(this, new EventArgs());
        }
        public event EventHandler<EventArgs> RootChanged;

    }
}